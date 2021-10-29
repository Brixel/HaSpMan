using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Domain;
using Domain.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Persistence;

using Queries.Enums;
using Queries.Members.ViewModels;

namespace Queries.Members.Handlers
{
    public record GetBankAccountInfos() : IRequest<IReadOnlyList<BankAccountInfo>>;
    public class BankAccountInfo
    {
        public BankAccountInfo(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class GetBankAccountInfosHandler : IRequestHandler<GetBankAccountInfos, IReadOnlyList<BankAccountInfo>>
    {
        private readonly IDbContextFactory<HaSpManContext> _contextFactory;

        public GetBankAccountInfosHandler(IDbContextFactory<HaSpManContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IReadOnlyList<BankAccountInfo>> Handle(GetBankAccountInfos request, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.BankAccounts
                .AsNoTracking()
                .Select(x => new BankAccountInfo(x.Id, x.Name))
                .ToListAsync(cancellationToken);
            
        }
    }

    public class AutocompleteMember
    {
        public AutocompleteMember(string name, Guid? memberId)
        {
            Name = name;
            MemberId = memberId;
        }
        public string Name { get; set; } = string.Empty;
        public Guid? MemberId { get; set; }
    };

    public class SearchMembersHandler : IRequestHandler<SearchMembersQuery, Paginated<MemberSummary>>
    {
        private readonly IDbContextFactory<HaSpManContext> _contextFactory;
        private readonly IMapper _mapper;

        public SearchMembersHandler(IDbContextFactory<HaSpManContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<Paginated<MemberSummary>> Handle(SearchMembersQuery request, CancellationToken cancellationToken)
        {
            var context = _contextFactory.CreateDbContext();
            var memberQueryable = context.Members
                .AsNoTracking()
                .Where(GetFilterCriteria(request.SearchString));

            var total = await memberQueryable.CountAsync(cancellationToken);

            memberQueryable = GetOrderedQueryable(request, memberQueryable);

            var memberSummaryQueryable = memberQueryable
                .ProjectTo<MemberSummary>(_mapper.ConfigurationProvider)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize);

            var items = await memberSummaryQueryable.ToListAsync();

            return new Paginated<MemberSummary>
            {
                Items = items,
                Total = total
            };
        }

        private static IQueryable<Member> GetOrderedQueryable(SearchMembersQuery request, IQueryable<Member> memberQueryable)
        {
            if (request.SortDirection == SortDirection.Ascending)
                memberQueryable = request.OrderBy switch
                {
                    MemberSummaryOrderOption.Address => memberQueryable
                        .OrderBy(m => m.Address.ZipCode)
                        .ThenBy(m => m.Address.City)
                        .ThenBy(m => m.Address.Street)
                        .ThenBy(m => m.Address.HouseNumber),
                    MemberSummaryOrderOption.Email => memberQueryable
                        .OrderBy(m => m.Email),
                    MemberSummaryOrderOption.Name => memberQueryable
                        .OrderBy(m => m.FirstName)
                        .ThenBy(m => m.LastName),
                    MemberSummaryOrderOption.PhoneNumber => memberQueryable
                        .OrderBy(m => m.PhoneNumber),
                    _ => memberQueryable
                        .OrderBy(m => m.FirstName)
                        .ThenBy(m => m.LastName)
                };

            if (request.SortDirection == SortDirection.Descending)
                memberQueryable = request.OrderBy switch
                {
                    MemberSummaryOrderOption.Address => memberQueryable
                        .OrderByDescending(m => m.Address.ZipCode)
                        .ThenBy(m => m.Address.City)
                        .ThenBy(m => m.Address.Street)
                        .ThenBy(m => m.Address.HouseNumber),
                    MemberSummaryOrderOption.Email => memberQueryable
                        .OrderByDescending(m => m.Email),
                    MemberSummaryOrderOption.Name => memberQueryable
                        .OrderByDescending(m => m.FirstName)
                        .ThenBy(m => m.LastName),
                    MemberSummaryOrderOption.PhoneNumber => memberQueryable
                        .OrderByDescending(m => m.PhoneNumber),
                    _ => memberQueryable
                        .OrderByDescending(m => m.FirstName)
                        .ThenBy(m => m.LastName)
                };
            return memberQueryable;
        }

        private static Expression<Func<Member, bool>> GetFilterCriteria(string searchString)
        {
            return m =>
                m.Address.City.ToLower().Contains(searchString) ||
                m.Address.Country.ToLower().Contains(searchString) ||
                m.Address.HouseNumber.ToLower().Contains(searchString) ||
                m.Address.Street.ToLower().Contains(searchString) ||
                m.Address.ZipCode.ToLower().Contains(searchString) ||
                m.Email.ToLower().Contains(searchString) ||
                m.FirstName.ToLower().Contains(searchString) ||
                m.LastName.ToLower().Contains(searchString) ||
                m.PhoneNumber.ToLower().Contains(searchString);
        }
    }
}