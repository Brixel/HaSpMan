using System;

using AutoMapper;

using Commands.Handlers.Transaction.AddCreditTransaction;
using Commands.Handlers.Transaction.AddDebitTransaction;

using Domain;

using Queries.Members.Handlers;
using Queries.Members.ViewModels;
using Queries.Transactions.ViewModels;

using Types;

using Web.Models;

namespace Web.MapperProfiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<TransactionForm, AddDebitTransactionCommand>()
                .ForCtorParam(nameof(AddDebitTransactionCommand.CounterPartyName), o => o.MapFrom(x => x.Member.Name))
                .ForCtorParam(nameof(AddDebitTransactionCommand.MemberId), o => o.MapFrom(x => x.Member.MemberId));
            CreateMap<TransactionForm, AddCreditTransactionCommand>()
                .ForCtorParam(nameof(AddDebitTransactionCommand.CounterPartyName), o => o.MapFrom(x => x.Member.Name))
                .ForCtorParam(nameof(AddDebitTransactionCommand.MemberId), o => o.MapFrom(x => x.Member.MemberId));
            ;
            CreateMap<TransactionTypeAmountForm, TransactionTypeAmount>();

            CreateMap<TransactionDetail, TransactionForm>()
                .ForMember(
                    x => x.ReceivedDateTime,
                    o =>
                        o.MapFrom(x => ToDateTime(x.ReceivedDateTime)))
                .ForMember(x => x.Member,
                    o => o.MapFrom(x => new AutocompleteMember(x.CounterPartyName, x.MemberId)));
                
            CreateMap<TransactionTypeAmount, TransactionTypeAmountForm>();
        }

        private static DateTime? ToDateTime(DateTimeOffset? dateTimeOffset)
        {
            if (dateTimeOffset == null)
                return null;

            return dateTimeOffset!.Value.DateTime;
        }
    }
}