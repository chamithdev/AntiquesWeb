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

        IPagedList<Product> SearchProductsCustom(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            int manufacturerId = 0,
            int storeId = 0,
            IList<int> vendorIds = null,
            int warehouseId = 0,
            ProductType? productType = null,
            bool visibleIndividuallyOnly = false,
            bool? featuredProducts = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int productTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchSku = true,
            bool searchProductTags = false,
            int languageId = 0,
            IList<int> filteredSpecs = null,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null,
             IList<string> customKeys = null,
            decimal sizeFrom = 0,
            decimal sizeTo = 0
            );

    }
}
