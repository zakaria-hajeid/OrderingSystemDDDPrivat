using FluentValidation;
using Ordering.Application.Order.Commands;

namespace Ordering.Application.Order.Validators
{
    public sealed class OrderCommandValidators: AbstractValidator<OrederCommand>
    {
        public OrderCommandValidators()
        {
            RuleFor(command => command.UserId)
            .NotEmpty()
            .WithMessage("The order identifier can't be empty.");
        }
    }
}
