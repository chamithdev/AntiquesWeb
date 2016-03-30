using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Customer;

namespace Nop.Web.Models.Customer
{
    public partial class RegisterModel
    {


        public string WebSite { get; set; }

        public string Skype { get; set; }

        public string Mobile { get; set; }

        public string Address { get; set; }
    }
}