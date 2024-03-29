using System.Linq.Expressions;

using Domain;

using Microsoft.EntityFrameworkCore;

using Persistence;

namespace Queries.Members;

public record MemberExistsByEmailQuery(string EmailAddress, Guid? ExcludeId = null) : IRequest<bool>;

public class MemberExistsByEmail : IRequestHandler<MemberExistsByEmailQuery, bool>
{
    private readonly HaSpManContext _context;

    public MemberExistsByEmail(HaSpManContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task<bool> Handle(MemberExistsByEmailQuery request, CancellationToken cancellationToken)
    {
        return _context.Members.AnyAsync(BuildQueryPredicate(request), cancellationToken);
    }

    private static Expression<Func<Member, bool>> BuildQueryPredicate(MemberExistsByEmailQuery request) => request.ExcludeId is null
        ? m => m.Email == request.EmailAddress
        : m => m.Id != request.ExcludeId && m.Email == request.EmailAddress;
}
