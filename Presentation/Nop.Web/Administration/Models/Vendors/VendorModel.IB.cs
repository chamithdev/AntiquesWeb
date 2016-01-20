using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System.ComponentModel;
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

    }
}