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

        public virtual IPagedList<Product> SearchProductsCustom(
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
            )
        {
            IList<int> filterableSpecificationAttributeOptionIds;
            return SearchProductsCustom(out filterableSpecificationAttributeOptionIds, false,
                pageIndex, pageSize, categoryIds, manufacturerId,
                storeId, vendorIds, warehouseId,
                productType, visibleIndividuallyOnly, featuredProducts,
                priceMin, priceMax, productTagId, keywords, searchDescriptions, searchSku,
                searchProductTags, languageId, filteredSpecs,
                orderBy, showHidden, overridePublished, customKeys, sizeFrom, sizeTo);
    }

        public virtual IPagedList<Product> SearchProductsCustom(
            out IList<int> filterableSpecificationAttributeOptionIds,
            bool loadFilterableSpecificationAttributeOptionIds = false,
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
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
            decimal sizeTo = 0)
        {
            filterableSpecificationAttributeOptionIds = new List<int>();

            //search by keyword
            bool searchLocalizedValue = false;
            if (languageId > 0)
            {
                if (showHidden)
                {
                    searchLocalizedValue = true;
                }
                else
                {
                    //ensure that we have at least two published languages
                    var totalPublishedLanguages = _languageService.GetAllLanguages().Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
                }
            }

            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            if (vendorIds != null && vendorIds.Contains(0))
                vendorIds.Remove(0);

            //Access control list. Allowed customer roles
            var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();

            //if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            //{
            //stored procedures are enabled and supported by the database. 
            //It's much faster than the LINQ implementation below 

            #region Use stored procedure

            //pass category identifiers as comma-delimited string
            string commaSeparatedCategoryIds = categoryIds == null ? "" : string.Join(",", categoryIds);


            //pass customer role identifiers as comma-delimited string
            string commaSeparatedAllowedCustomerRoleIds = string.Join(",", allowedCustomerRolesIds);

            string commaSeparatedVendorIds = string.Join(",", vendorIds);

            string commaSeparatedCustomKeys = string.Join(",", customKeys);


            //pass specification identifiers as comma-delimited string
            string commaSeparatedSpecIds = "";
            if (filteredSpecs != null)
            {
                ((List<int>)filteredSpecs).Sort();
                commaSeparatedSpecIds = string.Join(",", filteredSpecs);
            }

            //some databases don't support int.MaxValue
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            //prepare parameters
            var pCategoryIds = _dataProvider.GetParameter();
            pCategoryIds.ParameterName = "CategoryIds";
            pCategoryIds.Value = commaSeparatedCategoryIds != null ? (object)commaSeparatedCategoryIds : DBNull.Value;
            pCategoryIds.DbType = DbType.String;

            var pManufacturerId = _dataProvider.GetParameter();
            pManufacturerId.ParameterName = "ManufacturerId";
            pManufacturerId.Value = manufacturerId;
            pManufacturerId.DbType = DbType.Int32;

            var pStoreId = _dataProvider.GetParameter();
            pStoreId.ParameterName = "StoreId";
            pStoreId.Value = !_catalogSettings.IgnoreStoreLimitations ? storeId : 0;
            pStoreId.DbType = DbType.Int32;

            var pVendorId = _dataProvider.GetParameter();
            pVendorId.ParameterName = "VendorIds";
            pVendorId.Value = commaSeparatedVendorIds != null ? (object)commaSeparatedVendorIds : DBNull.Value;
            pVendorId.DbType = DbType.String;

            var pSizeFrom = _dataProvider.GetParameter();
            pSizeFrom.ParameterName = "SizeFrom";
            pSizeFrom.Value = (object)sizeFrom;
            pSizeFrom.DbType = DbType.Decimal;

            var pCustList = _dataProvider.GetParameter();
            pCustList.ParameterName = "CustomKeys";
            pCustList.Value = commaSeparatedCustomKeys != null ? (object)commaSeparatedCustomKeys : DBNull.Value; ;
            pCustList.DbType = DbType.String;

            var pSizeTo = _dataProvider.GetParameter();
            pSizeTo.ParameterName = "SizeTo";
            pSizeTo.Value = (object)sizeTo;
            pSizeTo.DbType = DbType.Decimal;

            var pWarehouseId = _dataProvider.GetParameter();
            pWarehouseId.ParameterName = "WarehouseId";
            pWarehouseId.Value = warehouseId;
            pWarehouseId.DbType = DbType.Int32;

            var pProductTypeId = _dataProvider.GetParameter();
            pProductTypeId.ParameterName = "ProductTypeId";
            pProductTypeId.Value = productType.HasValue ? (object)productType.Value : DBNull.Value;
            pProductTypeId.DbType = DbType.Int32;

            var pVisibleIndividuallyOnly = _dataProvider.GetParameter();
            pVisibleIndividuallyOnly.ParameterName = "VisibleIndividuallyOnly";
            pVisibleIndividuallyOnly.Value = visibleIndividuallyOnly;
            pVisibleIndividuallyOnly.DbType = DbType.Int32;

            var pProductTagId = _dataProvider.GetParameter();
            pProductTagId.ParameterName = "ProductTagId";
            pProductTagId.Value = productTagId;
            pProductTagId.DbType = DbType.Int32;

            var pFeaturedProducts = _dataProvider.GetParameter();
            pFeaturedProducts.ParameterName = "FeaturedProducts";
            pFeaturedProducts.Value = featuredProducts.HasValue ? (object)featuredProducts.Value : DBNull.Value;
            pFeaturedProducts.DbType = DbType.Boolean;

            var pPriceMin = _dataProvider.GetParameter();
            pPriceMin.ParameterName = "PriceMin";
            pPriceMin.Value = priceMin.HasValue ? (object)priceMin.Value : DBNull.Value;
            pPriceMin.DbType = DbType.Decimal;

            var pPriceMax = _dataProvider.GetParameter();
            pPriceMax.ParameterName = "PriceMax";
            pPriceMax.Value = priceMax.HasValue ? (object)priceMax.Value : DBNull.Value;
            pPriceMax.DbType = DbType.Decimal;

            var pKeywords = _dataProvider.GetParameter();
            pKeywords.ParameterName = "Keywords";
            pKeywords.Value = keywords != null ? (object)keywords : DBNull.Value;
            pKeywords.DbType = DbType.String;

            var pSearchDescriptions = _dataProvider.GetParameter();
            pSearchDescriptions.ParameterName = "SearchDescriptions";
            pSearchDescriptions.Value = searchDescriptions;
            pSearchDescriptions.DbType = DbType.Boolean;

            var pSearchSku = _dataProvider.GetParameter();
            pSearchSku.ParameterName = "SearchSku";
            pSearchSku.Value = searchSku;
            pSearchSku.DbType = DbType.Boolean;

            var pSearchProductTags = _dataProvider.GetParameter();
            pSearchProductTags.ParameterName = "SearchProductTags";
            pSearchProductTags.Value = searchProductTags;
            pSearchProductTags.DbType = DbType.Boolean;

            var pUseFullTextSearch = _dataProvider.GetParameter();
            pUseFullTextSearch.ParameterName = "UseFullTextSearch";
            pUseFullTextSearch.Value = _commonSettings.UseFullTextSearch;
            pUseFullTextSearch.DbType = DbType.Boolean;

            var pFullTextMode = _dataProvider.GetParameter();
            pFullTextMode.ParameterName = "FullTextMode";
            pFullTextMode.Value = (int)_commonSettings.FullTextMode;
            pFullTextMode.DbType = DbType.Int32;

            var pFilteredSpecs = _dataProvider.GetParameter();
            pFilteredSpecs.ParameterName = "FilteredSpecs";
            pFilteredSpecs.Value = commaSeparatedSpecIds != null ? (object)commaSeparatedSpecIds : DBNull.Value;
            pFilteredSpecs.DbType = DbType.String;

            var pLanguageId = _dataProvider.GetParameter();
            pLanguageId.ParameterName = "LanguageId";
            pLanguageId.Value = searchLocalizedValue ? languageId : 0;
            pLanguageId.DbType = DbType.Int32;

            var pOrderBy = _dataProvider.GetParameter();
            pOrderBy.ParameterName = "OrderBy";
            pOrderBy.Value = (int)orderBy;
            pOrderBy.DbType = DbType.Int32;

            var pAllowedCustomerRoleIds = _dataProvider.GetParameter();
            pAllowedCustomerRoleIds.ParameterName = "AllowedCustomerRoleIds";
            pAllowedCustomerRoleIds.Value = commaSeparatedAllowedCustomerRoleIds;
            pAllowedCustomerRoleIds.DbType = DbType.String;

            var pPageIndex = _dataProvider.GetParameter();
            pPageIndex.ParameterName = "PageIndex";
            pPageIndex.Value = pageIndex;
            pPageIndex.DbType = DbType.Int32;

            var pPageSize = _dataProvider.GetParameter();
            pPageSize.ParameterName = "PageSize";
            pPageSize.Value = pageSize;
            pPageSize.DbType = DbType.Int32;

            var pShowHidden = _dataProvider.GetParameter();
            pShowHidden.ParameterName = "ShowHidden";
            pShowHidden.Value = showHidden;
            pShowHidden.DbType = DbType.Boolean;

            var pOverridePublished = _dataProvider.GetParameter();
            pOverridePublished.ParameterName = "OverridePublished";
            pOverridePublished.Value = overridePublished != null ? (object)overridePublished.Value : DBNull.Value;
            pOverridePublished.DbType = DbType.Boolean;

            var pLoadFilterableSpecificationAttributeOptionIds = _dataProvider.GetParameter();
            pLoadFilterableSpecificationAttributeOptionIds.ParameterName = "LoadFilterableSpecificationAttributeOptionIds";
            pLoadFilterableSpecificationAttributeOptionIds.Value = loadFilterableSpecificationAttributeOptionIds;
            pLoadFilterableSpecificationAttributeOptionIds.DbType = DbType.Boolean;

            var pFilterableSpecificationAttributeOptionIds = _dataProvider.GetParameter();
            pFilterableSpecificationAttributeOptionIds.ParameterName = "FilterableSpecificationAttributeOptionIds";
            pFilterableSpecificationAttributeOptionIds.Direction = ParameterDirection.Output;
            pFilterableSpecificationAttributeOptionIds.Size = int.MaxValue - 1;
            pFilterableSpecificationAttributeOptionIds.DbType = DbType.String;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            //invoke stored procedure
            var products = _dbContext.ExecuteStoredProcedureList<Product>(
                "ProductLoadAllPagedCustom",
                pCategoryIds,
                pManufacturerId,
                pStoreId,
                pVendorId,
                pWarehouseId,
                pProductTypeId,
                pVisibleIndividuallyOnly,
                pProductTagId,
                pFeaturedProducts,
                pPriceMin,
                pPriceMax,
                pKeywords,
                pSearchDescriptions,
                pSearchSku,
                pSearchProductTags,
                pUseFullTextSearch,
                pFullTextMode,
                pFilteredSpecs,
                pLanguageId,
                pOrderBy,
                pAllowedCustomerRoleIds,
                pPageIndex,
                pPageSize,
                pShowHidden,
                pOverridePublished,
                pLoadFilterableSpecificationAttributeOptionIds,
                pFilterableSpecificationAttributeOptionIds,
                pTotalRecords,
                pSizeFrom,
                pSizeTo,
                pCustList
                );
            //get filterable specification attribute option identifier
            string filterableSpecificationAttributeOptionIdsStr = (pFilterableSpecificationAttributeOptionIds.Value != DBNull.Value) ? (string)pFilterableSpecificationAttributeOptionIds.Value : "";
            if (loadFilterableSpecificationAttributeOptionIds &&
                !string.IsNullOrWhiteSpace(filterableSpecificationAttributeOptionIdsStr))
            {
                filterableSpecificationAttributeOptionIds = filterableSpecificationAttributeOptionIdsStr
                   .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                   .Select(x => Convert.ToInt32(x.Trim()))
                   .ToList();
            }
            //return products
            int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return new PagedList<Product>(products, pageIndex, pageSize, totalRecords);

        }

            #endregion
    }
           
}
