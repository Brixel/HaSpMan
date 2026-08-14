using AutoMapper;

using Commands.Handlers.BankAccount.AddBankAccount;

using Domain;

using Queries.BankAccounts;

using Web.Models;

namespace Web.MapperProfiles;

public class BankAccountProfile : Profile
{
    public BankAccountProfile()
    {
        
        CreateMap<BankAccountDetail, BankAccountForm>();

        CreateMap<BankAccount, BankAccountDetail>()
            .ForCtorParam(nameof(BankAccountDetail.Id), o => o.MapFrom(src => src.Id))
            .ForCtorParam(nameof(BankAccountDetail.AccountNumber), o => o.MapFrom(src => src.AccountNumber))
            .ForCtorParam(nameof(BankAccountDetail.Name), o => o.MapFrom(src => src.Name));

        CreateMap<BankAccount, BankAccountDetailWithTotal>()
            .ForCtorParam(nameof(BankAccountDetailWithTotal.TotalAmount), o => o.MapFrom(src => src.Totals == null ? 0 : src.Totals.Amount))
            .ForCtorParam(nameof(BankAccountDetailWithTotal.NumberOfTransactions), o
                => o.MapFrom(src => src.Totals == null ? 0 : src.Totals.NumberOfTransactions));
    }
}