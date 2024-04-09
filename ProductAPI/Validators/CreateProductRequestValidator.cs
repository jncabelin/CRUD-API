using System;
using FluentValidation;
using ProductAPI.DTOs.Product;  
using static ProductAPI.Constants.ProductMessage;

namespace ProductAPI.Validators
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(NameIsRequired);
            RuleFor(x => x.Name)
                .Length(3, 50)
                .WithMessage(NameLength);
            RuleFor(x => x.Brand)
                .NotEmpty()
                .WithMessage(BrandIsRequired);
            RuleFor(x => x.Brand)
                .Length(3, 20)
                .WithMessage(BrandLength);
            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage(PriceIsRequired)
                .GreaterThan(0)
                .WithMessage(PriceGreaterThan);
        }
    }
}

