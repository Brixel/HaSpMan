using MediatR;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

using MudBlazor;

using Queries.FinancialYears;
using Queries.Members.Handlers.AutocompleteMember;
using Queries.Members.Handlers.GetBankAccountInfos;
using Queries.Members.Handlers.GetMemberById;
using Queries.Members.ViewModels;
using Queries.Transactions.GetAttachment;

using Types;

using Web.Extensions;
using Web.Models;

namespace Web.Pages.Transactions;

public partial class TransactionForm : ComponentBase
{
    [Inject]
    private IWebHostEnvironment Environment { get; set; } = default!;

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    [Inject]
    private IMediator Mediator { get; set; } = default!;

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Parameter]
    public Models.TransactionForm Transaction { get; set; } = new();

    [Parameter]
    public bool IsSearchForMembership { get; set; }

    [Parameter]
    public TransactionTypeGroup TransactionTypeGroup { get; set; }

    [Parameter]
    public EventCallback<TransactionTypeGroup> TransactionTypeGroupChanged { get; set; }

    [Parameter]
    public bool IsNew { get; set; }

    public string? FileErrorMessage { get; set; }

    private IReadOnlyList<BankAccountInfo> _bankAccounts = new List<BankAccountInfo>();

    private IReadOnlyList<TransactionType> _transactionTypes = new List<TransactionType>();

    private IReadOnlyList<FinancialYear> _financialYears = new List<FinancialYear>();

    private DateTime? _newExpirationDate;

    private int AmountOfMonths { get; set; }

    private MemberDetail? SelectedMember { get; set; }

    private bool CanExtendMembership => Transaction.TransactionTypeAmounts
        .Any(x => x.TransactionType == Types.TransactionType.DebitMemberFee) &&
        Transaction.Counterparty.MemberId.HasValue &&
        SelectedMember != null &&
        SelectedMember.MembershipFee != 0;

    private DateTime? MinReceivedDateTime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetAllBankAccounts();
        SetTransactionTypes();
    }

    protected override async Task OnParametersSetAsync()
    {
        await TrySetFinancialYearId();
        await SetFinancialYears();
    }

    private void SetTransactionTypes()
    {
        _transactionTypes = TransactionTypeGroup.GetScopedTransactionTypes();
    }

    private MudDatePicker _picker = new();
    private readonly int _maxAllowedSize = 1024 * 1024 * 10;
    private readonly IReadOnlyList<string> _allowedFileTypes = new List<string> {
        "pdf", "doc", "docx", "jpg", "jpeg", "png", "gif" };

    private async Task<IEnumerable<AutocompleteCounterparty>> SearchForMembers(string searchString, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            return new List<AutocompleteCounterparty>();
        }
        var response = await Mediator.Send(new AutocompleteCounterpartyQuery(searchString, true), cancellationToken);
        return response.Counterparties;
    }

    private async Task<IEnumerable<AutocompleteCounterparty>> SearchForNonMembers(string searchString, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new AutocompleteCounterpartyQuery(searchString, false), cancellationToken);

        return response.Counterparties;
    }

    private async Task GetAllBankAccounts()
    {
        _bankAccounts = await Mediator.Send(new GetBankAccountInfos());

        if (_bankAccounts.Count == 1 && Transaction.BankAccountId == null)
        {
            Transaction.BankAccountId = _bankAccounts[0].Id;
        }
    }

    private async Task OpenAddBankAccountDialog()
    {
        var dialog = await DialogService.ShowAsync<AddBankAccountDialog>("Add bank account");
        var result = await dialog.Result;
        if ((result is { Canceled: true, Data: true }))
        {
            await GetAllBankAccounts();
        }
    }

    private readonly Converter<AutocompleteCounterparty> _counterPartyConverter = new()
    {
        SetFunc = value => value?.Name ?? "",
        GetFunc = text => new AutocompleteCounterparty(text ?? "", null)
    };

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        FileErrorMessage = string.Empty;
        var entries = e.GetMultipleFiles();
        var notAllowedFileTypes = entries.Any(x => !_allowedFileTypes.Any(fileType => x.Name.EndsWith(fileType)));
        if (notAllowedFileTypes)
        {
            var joinedFileTypes = string.Join(", ", _allowedFileTypes);
            FileErrorMessage = $"Some of the files are not of the supported file types ({joinedFileTypes})";
            return;
        }
        var overMaxFileSize = entries.Any(x => x.Size > _maxAllowedSize);
        if (overMaxFileSize)
        {
            FileErrorMessage = "Some of the files are too large (max 10mb)";
            return;
        }
        var existingFileNames = Transaction.TransactionAttachments.Select(x => x.FileName).ToList();
        var newFiles = entries.Where(x => existingFileNames.All(fileName => fileName != x.Name));

        foreach (var file in newFiles)
        {
            var trustedFileNameForFileStorage = Path.GetRandomFileName();
            var directory = Path.Combine(Environment.ContentRootPath,
            Environment.EnvironmentName, "unsafe_uploads");
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, trustedFileNameForFileStorage);

            await using FileStream fs = new(path, FileMode.Create);
            await file.OpenReadStream(_maxAllowedSize).CopyToAsync(fs);
            var transaction = new NewTransactionAttachment(file.Name, file.ContentType, path);
            Transaction.NewTransactionAttachments.Add(transaction);
        }
    }

    private async Task Download(string attachmentFileName)
    {
        var attachment = await Mediator.Send(new GetAttachmentQuery(Transaction.Id, attachmentFileName));
        var fileStream = new MemoryStream(attachment.Bytes);
        using var streamRef = new DotNetStreamReference(fileStream);
        await JS.InvokeVoidAsync("downloadFileFromStream", attachment.Name, streamRef); // THIS FEELS SOOOO DIRTY
    }

    private async Task OnChangeMember(AutocompleteCounterparty counterparty)
    {
        Transaction.Counterparty = counterparty;
        await CalculateMembershipExpirationDate();
    }

    private async Task OnChangeTypeGroup(TransactionTypeGroup typeGroup)
    {
        if (TransactionTypeGroup == typeGroup)
            return;

        TransactionTypeGroup = typeGroup;
        await TransactionTypeGroupChanged.InvokeAsync(typeGroup);
        SetTransactionTypes();
        foreach (var transactionTypeAmount in Transaction.TransactionTypeAmounts)
            transactionTypeAmount.TransactionType = null;
    }

    private async Task OnAmountChanged(TransactionTypeAmountForm transactionTypeAmount, decimal? value)
    {
        transactionTypeAmount.Amount = value;
        await CalculateMembershipExpirationDate();
    }


    private async Task CalculateMembershipExpirationDate()
    {
        if (!Transaction.Counterparty.MemberId.HasValue)
        {
            return;
        }
        var memberId = Transaction.Counterparty.MemberId.Value;
        var member = await Mediator.Send(new GetMemberByIdQuery(memberId));
        SelectedMember = member;
        var transactionTypeForMembershipFee = Transaction.TransactionTypeAmounts
            .SingleOrDefault(x => x.TransactionType == TransactionType.DebitMemberFee);

        if (transactionTypeForMembershipFee != null &&
            transactionTypeForMembershipFee.Amount is not null or 0 &&
            SelectedMember.MembershipExpiryDate.HasValue && SelectedMember.MembershipFee != 0)
        {
            AmountOfMonths = (int)Math.Floor((double)transactionTypeForMembershipFee.Amount / SelectedMember.MembershipFee);
            _newExpirationDate = SelectedMember.MembershipExpiryDate.Value.AddMonths(AmountOfMonths).Date;
            Transaction.NewMembershipExpirationDate = _newExpirationDate;
        }
    }

    private void SetMinReceivedDate(Guid? financialYearId)
    {
        Transaction.FinancialYearId = financialYearId;
        MinReceivedDateTime = financialYearId is null
            ? null
            : _financialYears.Single(f => f.Id == financialYearId).StartDateTimeOffset.DateTime;
    }

    private async Task TrySetFinancialYearId()
    {
        if (Transaction.Id == default)
            return;

        var financialYear = await Mediator.Send(new GetFinancialYearByTransactionId(Transaction.Id))
            ?? throw new Exception($"Couldn't get financial year id for transaction with id {Transaction.Id}");

        Transaction.FinancialYearId = financialYear.Id;
        await Task.Delay(1000);
    }

    private async Task SetFinancialYears()
    {
        _financialYears = await Mediator.Send(new GetFinancialYearsQuery());
        if (Transaction.FinancialYearId is not null)
            SetMinReceivedDate(Transaction.FinancialYearId);
        else
            SetMinReceivedDate(_financialYears.OrderByDescending(f => f.StartDateTimeOffset).First().Id);
    }
}