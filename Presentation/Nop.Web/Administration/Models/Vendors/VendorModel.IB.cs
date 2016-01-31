using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System.ComponentModel;
using Nop.Core.Domain.Media;
using System.ComponentModel.DataAnnotations;

namespace Nop.Admin.Models.Vendors
{
    public partial class VendorModel
    {


        [DisplayName("Show on Homepage")]
        public bool ShowOnHomePage { get; set; }

        [DisplayName("Country")]
        public string Country { get; set; }
        [DisplayName("Town")]
        public string City { get; set; }
        [DisplayName("Web Site")]
        public string Web { get; set; }

        public int PictureId { get; set; }

        public Picture MainPicture { get; set; }

        public VenddorPictureModel MainPictureModel { get; set; }

        public List<VenddorProductModel> Products { get; set; }

        public bool DisplayActive { get; set; }

        public partial class VenddorPictureModel
        {
            public int ProductId { get; set; }

            [UIHint("Picture")]
            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.Picture")]
            public int PictureId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.Picture")]
            public string PictureUrl { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.OverrideAltAttribute")]
            [AllowHtml]
            public string OverrideAltAttribute { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.OverrideTitleAttribute")]
            [AllowHtml]
            public string OverrideTitleAttribute { get; set; }
        }

        public partial class VenddorProductModel
        {
            public int Id { get; set; }
            public int ProductId { get; set; }

            public VenddorPictureModel ProductPicture { get; set; }

            public string ProductName { get; set; }

            public string ProductPrice { get; set; }

            public string CircaDate { get; set; }

            public int DisplayOrder { get; set; }

            public int TotalCount { get; set; }

        }
        public partial class ProductPagingModel
        {
            public int VendorId { get; set; }
            public int PageIndex { get; set; }
        }


    }

    
    
}