using Domain.Interfaces;

namespace Commands.Handlers.Member.ExtendMembership;

public class ExtendMembershipHandler : IRequestHandler<ExtendMembershipCommand>
{
    private readonly IMemberRepository _repository;

    public ExtendMembershipHandler(IMemberRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(ExtendMembershipCommand request, CancellationToken cancellationToken)
    {
        var member = await _repository.GetById(request.Id)
            ?? throw new ArgumentException($"No member found by Id {request.Id}", nameof(request.Id));
        
        member.ChangeMembershipExpiryDate(request.NewMembershipExpirationDate, request.PerformingUser);

        await _repository.Save(cancellationToken);
    }
}
