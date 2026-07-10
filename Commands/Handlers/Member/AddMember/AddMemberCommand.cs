using FluentValidation;

using Types;

namespace Commands.Handlers.Member.AddMember;

public record AddMemberCommand(
    string FirstName,
    string LastName,
    Address Address,
    string Email,
    string PhoneNumber,
    double MembershipFee,
    DateTimeOffset MembershipExpiryDate,
    string PerformingUser
) : IRequest<Guid>;

public class AddMemberCommandValidator : AbstractValidator<AddMemberCommand>
{
    public AddMemberCommandValidator()
    {
        RuleFor(x => x.FirstName)
           .NotEmpty()
           .MaximumLength(50);

        RuleFor(x => x.LastName)
           .NotEmpty()
           .MaximumLength(50);

        RuleFor(x => x.Email)
           .NotEmpty().When(x => string.IsNullOrWhiteSpace(x.PhoneNumber))
           .MaximumLength(100)
           .EmailAddress();

        RuleFor(x => x.PhoneNumber)
           .NotEmpty().When(x => string.IsNullOrWhiteSpace(x.Email))
           .MaximumLength(50);

        RuleFor(x => x.Address)
           .SetValidator(new AddMemberCommandAddressValidator());

        RuleFor(x => x.MembershipFee)
           .GreaterThanOrEqualTo(0);

        RuleFor(x => x.PerformingUser)
            .NotEmpty();
    }

    public class AddMemberCommandAddressValidator : AbstractValidator<Address>
    {
        public AddMemberCommandAddressValidator()
        {
            RuleFor(x => x.Street)
               .NotEmpty()
               .MaximumLength(200);

            RuleFor(x => x.City)
               .NotEmpty()
               .MaximumLength(50);

            RuleFor(x => x.Country)
               .NotEmpty()
               .MaximumLength(50);

            RuleFor(x => x.ZipCode)
               .NotEmpty()
               .MaximumLength(10);

            RuleFor(x => x.HouseNumber)
               .NotEmpty()
               .MaximumLength(15);
        }
    }
}

