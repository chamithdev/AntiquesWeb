﻿using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public partial class VendorModel : BaseNopEntityModel
    {
        public VendorModel()
        {
            Products = new List<ProductOverviewModel>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public bool AllowCustomersToContactVendors { get; set; }

        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public IList<ProductOverviewModel> Products { get; set; }

        public string ImageUrl { get; set; }

        public string ImageUrl2 { get; set; }

        public string City { get; set; }
        public string Coutry { get; set; }
        public string Web { get; set; }

        /// <summary>
        /// Phone number of the first customer attached to this Vendor
        /// </summary>
        public string PhoneOfTheFirstCustomer { get; set; }
    }
}