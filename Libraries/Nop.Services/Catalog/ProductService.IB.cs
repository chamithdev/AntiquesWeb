using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    public partial class ProductService
    {

        /// <summary>
        /// Gets all products displayed on the home page
        /// </summary>
        /// <returns>Products</returns>
        /// 
        public virtual IList<Product> GetLatestProductsDisplayedOnHomePage()
        {
            var baseDate = DateTime.Now.AddDays(-31);
            var query = from p in _productRepository.Table
                        orderby p.DisplayOrder, p.Name
                        where p.Published &&
                        !p.Deleted &&
                        p.ShowOnHomePage
                        && p.CreatedOnUtc >= baseDate
                        select p;
            var products = query.ToList();
            return products;
        }

        public virtual IList<Product> GetAllProductsForVendorId(int vendorId)
        {
            var query = _productRepository.Table;
            query = query.Where(x => x.VendorId == vendorId);
            
            query = query.Where(x => !x.Deleted);
            query = query.OrderBy(x => x.DisplayOrder);

            var products = query.ToList();

            return products;
        }
    }
}
