using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Models.Customers;
using Nop.Admin.Models.Discounts;
using Nop.Admin.Models.Stores;
using Nop.Admin.Validators.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using Nop.Core.Domain.Catalog;

namespace Nop.Admin.Models.Catalog
{
    public partial class ProductModel
    {


        //Circa Date

        public string CircaDate { get; set; }

        public string Material { get; set; }

        public string Color { get; set; }

        public string Country { get; set; }

        public string DesignBy { get; set; }

        public string Condition { get; set; }

        public bool TermsCondtion { get; set; }


        public int Rank { get; set; }

        public string Style { get; set; }

        private IList<SelectListItem> _styles;

        public IList<SelectListItem> Styles
        {
            get { return _styles ?? new List<SelectListItem>(); ; }
            set { _styles = value; }
        }

        private IList<SelectListItem> _circaDates;
        private IList<SelectListItem> _materials;

        public IList<SelectListItem> CircaDates
        {
            get { return _circaDates ?? new List<SelectListItem>(); ; }
            set { _circaDates = value; }
        }

        public IList<SelectListItem> Materials
        {
            get { return _materials ?? new List<SelectListItem>(); ; }
            set { _materials = value; }
        }

        public List<CategoryModel> Categories { get; set; }
        
        public string City { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.Category")]
        [AllowHtml]
        public string Category { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.FileNameTagKeyWord")]
        [AllowHtml]
        public string FileNameTagKeyWord { get; set; }
    }
}