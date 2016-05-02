﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        IList<Product> GetLatestProducts(Expression<Func<Product, bool>> predicate);

        /// <summary>
        /// Get All Products for given vendor id.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        IList<Product> GetAllProductsForVendorId(int vendorId, string orderBy = "", string searchTerm = "");


        IPagedList<Product> SearchProductsCustom
            (
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
            decimal sizeTo = 0,
            decimal varience = 0,
            string circaDateFrom = "",
            string circaDateTo = "",
            string color = "",
            string designBy = "",
            decimal widthFrom = 0,
            decimal widthTo = 0
            );

        int GetMaxDisplayOrder(int vendorId);

        int GetMaxDisplayOrderUnsold(int vendorId);

        int GetMinDisplayOrder(int vendorId);

        void RearrangeDisplayOrder(int productId, int displayOrder);


        decimal MaxAvalablePrice();

        decimal MinAvalablePrice();

        decimal MaxAvalableHeight();

        decimal MinAvalableHeight();

        decimal MaxAvalableWidth();

        decimal MinAvalableWidth();

        IList<string> GetColorList();

        IList<string> GetDesignerList();

    }
}
