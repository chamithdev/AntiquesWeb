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

            RuleFor(x => x.DesignBy).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.DesignBy.Required"));

            RuleFor(x => x.ShortDescription).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.ShortDescription.Required"));

            RuleFor(x => x.Price).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Price.Required"));

            RuleFor(x => x.CircaDate).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.CircaDate.Required"));

            RuleFor(x => x.Country).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Country.Required"));

            RuleFor(x => x.CircaDate)
                .Length(4)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.FourDigitCircaDate.Length"));
        }
    }
}