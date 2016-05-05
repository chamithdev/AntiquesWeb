using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Catalog
{
    public class ProductValidator : BaseNopValidator<ProductModel>
    {
        public ProductValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Name.Required"));

            RuleFor(x => x.Name).Length(1, 400);

            RuleFor(x => x.MetaKeywords).Length(0, 400);

            RuleFor(x => x.MetaDescription).Length(0, 4096);

            RuleFor(x => x.ShortDescription).Length(0, 4096);

            RuleFor(x => x.Color).Length(0, 50);

            RuleFor(x => x.Country).Length(0, 50);

            RuleFor(x => x.DesignBy).Length(0, 200);

            RuleFor(x => x.Condition).Length(0, 400);

            RuleFor(x => x.CircaDate)
                .Length(4)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.FourDigitCircaDate.Length"));
        }
    }
}