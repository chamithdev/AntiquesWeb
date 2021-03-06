﻿using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;
using Nop.Core.Domain.Vendors;

namespace Nop.Web.Models.Catalog
{
    public partial class ProductOverviewModel
    {
        public IList<Vendor> Vendors { get; set; }

        public VendorModel Vendor { get; set; }

        public string CircaDate { get; set; }     

        public string DesignBy { get; set; }

        public string Style { get; set; }

        public int StockQuantity { get; set; }
    }
}