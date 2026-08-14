using Domain.Interfaces;

using Microsoft.EntityFrameworkCore;

using Persistence;

namespace Commands.Handlers.Member.EditMember;

public class EditMemberHandler : IRequestHandler<EditMemberCommand>
{
    private readonly IMemberRepository _memberRepository;
    private readonly HaSpManContext _dbContext;

    public EditMemberHandler(IMemberRepository memberRepository, HaSpManContext dbContext)
    {
        _memberRepository = memberRepository;
        _dbContext = dbContext;
    }

    public async Task Handle(EditMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetById(request.Id)
            ?? throw new ArgumentException($"No member found by Id {request.Id}", nameof(request.Id));

        await EnsureMemberIsNotBecomeDuplicate(request, cancellationToken);

        var performingUser = request.PerformingUser;
        member.ChangeAddress(request.Address, performingUser);
        member.ChangeEmail(request.Email, performingUser);
        member.ChangeMembershipExpiryDate(request.MembershipExpiryDate, performingUser);
        member.ChangeMembershipFee(request.MembershipFee, performingUser);
        member.ChangeName(request.FirstName, request.LastName, performingUser);
        member.ChangePhoneNumber(request.PhoneNumber, performingUser);

        await _memberRepository.Save(cancellationToken);
    }

    private async Task EnsureMemberIsNotBecomeDuplicate(EditMemberCommand command, CancellationToken ct)
    {
        var memberExistsByEmail = await _dbContext.Members
            .AsNoTracking()
            .AnyAsync(m => m.Id != command.Id && m.Email == command.Email, ct);

        if (memberExistsByEmail)
            throw new InvalidOperationException("Can't add member with same email address as existing member");

        var memberExistsByNameAndAddress = await _dbContext.Members
            .AsNoTracking()
            .AnyAsync(m =>
                m.Id != command.Id
                && m.FirstName == command.FirstName
                && m.LastName == command.LastName
                && m.Address.Street == command.Address.Street
                && m.Address.City == command.Address.City
                && m.Address.Country == command.Address.Country
                && m.Address.ZipCode == command.Address.ZipCode
                && m.Address.HouseNumber == command.Address.HouseNumber, ct);

        if (memberExistsByNameAndAddress)
            throw new InvalidOperationException("Can't add member with same name and address as existing member");
    }
}
