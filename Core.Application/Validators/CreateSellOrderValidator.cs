using Core.Application.Commands;
using FluentValidation;

namespace Core.Application.Validators;

public class CreateSellOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateSellOrderValidator()
    {
        RuleFor(request => request.Crypto).IsInEnum();
        RuleFor(request => request.CryptoAmount).GreaterThan(0);
        RuleFor(request => request.Fiat).IsInEnum();
        RuleFor(request => request.CryptoToFiatExchangeRate).GreaterThan(0);
        RuleFor(request => request.PaymentMethodInfo).NotEmpty();
    }
}