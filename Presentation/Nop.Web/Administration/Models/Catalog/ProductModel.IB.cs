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

        public string City { get; set; }
    }
}