using AutoMapper;

using Commands.Extensions;
using Commands.Services;

using Domain.Interfaces;

using MediatR;

namespace Commands.Handlers;

public class
    EditMemberHandler : IRequestHandler<EditMemberCommand>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMapper _mapper;
    private readonly IUserAccessor _userAccessor;

    public EditMemberHandler(IMemberRepository memberRepository, IMapper mapper, IUserAccessor userAccessor)
    {
        _memberRepository = memberRepository;
        _mapper = mapper;
        _userAccessor = userAccessor;
    }

    public async Task<Unit> Handle(EditMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetById(request.Id);

        if (member == null)
            throw new ArgumentException($"No member found by Id {request.Id}", nameof(request.Id));

        var performingUser = _userAccessor.User.GetName()!;
        member.ChangeAddress(request.Address, performingUser);
        member.ChangeEmail(request.Email, performingUser);
        member.ChangeMembershipExpiryDate(request.MembershipExpiryDate, performingUser);
        member.ChangeMembershipFee(request.MembershipFee, performingUser);
        member.ChangeName(request.FirstName, request.LastName, performingUser);
        member.ChangePhoneNumber(request.PhoneNumber, performingUser);

        await _memberRepository.Save(cancellationToken);

        return Unit.Value;
    }
}
