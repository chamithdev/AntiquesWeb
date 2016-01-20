using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;


namespace Nop.Services.Catalog
{
    public partial interface IProductService
    {

        /// <summary>
        /// get latest products
        /// </summary>
        /// <returns></returns>
        IList<Product> GetLatestProductsDisplayedOnHomePage();



    }
}
