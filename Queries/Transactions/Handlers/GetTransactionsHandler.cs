﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Persistence;

using Queries.Enums;
using Queries.Transactions.ViewModels;

namespace Queries.Transactions.Handlers
{
    public class GetTransactionsHandler : IRequestHandler<GetTransactionQuery, Paginated<TransactionSummary>>
    {
        private readonly HaSpManContext _context;
        private readonly IMapper _mapper;

        public GetTransactionsHandler(HaSpManContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Paginated<TransactionSummary>> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
        {
            var transactions = _context.Transactions.AsNoTracking()
                .Where(GetFilterCriteria(request.SearchString));

            var totalCount = await transactions.CountAsync(cancellationToken);

            transactions = GetOrderedQueryable(request, transactions);

            var transactionViewModels = 
                transactions
                    .ProjectTo<TransactionSummary>(_mapper.ConfigurationProvider)
                    .Skip(request.PageIndex * request.PageSize)
                    .Take(request.PageSize);

            var items = await transactionViewModels.ToListAsync(cancellationToken);

            return new Paginated<TransactionSummary>()
            {
                Items = items,
                Total = totalCount
            };

        }

        private IQueryable<Transaction> GetOrderedQueryable(GetTransactionQuery request, IQueryable<Transaction> transactions)
        {
            transactions = request.SortDirection switch
            {
                SortDirection.Ascending => request.OrderBy switch
                {
                    TransactionSummaryOrderOption.From => transactions.OrderBy(x => x.CounterParty.Name),
                    TransactionSummaryOrderOption.Amount => transactions.OrderBy(x => x.Amount),
                    TransactionSummaryOrderOption.ReceivedDateTime => transactions.OrderBy(x => x.ReceivedDateTime),
                    _ => transactions.OrderBy(x => x.DateFiled)
                },
                SortDirection.Descending => request.OrderBy switch
                {
                    TransactionSummaryOrderOption.From => transactions.OrderByDescending(x => x.CounterParty.Name),
                    TransactionSummaryOrderOption.Amount => transactions.OrderByDescending(x => x.Amount),
                    TransactionSummaryOrderOption.ReceivedDateTime => transactions.OrderByDescending(x =>
                        x.ReceivedDateTime),
                    _ => transactions.OrderByDescending(x => x.DateFiled)
                },
                _ => transactions
            };

            return transactions;
        }

        public Expression<Func<Transaction, bool>> GetFilterCriteria(string searchString)
        {
            return t => t.CounterParty.Name.ToLower().Contains(searchString) ||
                        t.Amount.ToString().Contains(searchString);
        } 
    }

    public record GetTransactionQuery(string SearchString, int PageIndex, int PageSize,
        TransactionSummaryOrderOption OrderBy, SortDirection SortDirection) : IRequest<Paginated<TransactionSummary>>;

    public enum TransactionSummaryOrderOption
    {
        From,
        ReceivedDateTime,
        Amount
    }
}