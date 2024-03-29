﻿using Queries.Enums;
using Queries.Members.ViewModels;

namespace Queries.Members.Handlers.SearchMembers;

public record SearchMembersQuery(
    string SearchString,
    bool ShowActive,
    bool ShowInactive,
    int PageIndex,
    int PageSize,
    MemberSummaryOrderOption OrderBy,
    SortDirection SortDirection
) : IRequest<Paginated<MemberSummary>>;

public enum MemberSummaryOrderOption
{
    Name,
    Address,
    Email,
    PhoneNumber,
    IsActive,
    MembershipExpiryDate
}
