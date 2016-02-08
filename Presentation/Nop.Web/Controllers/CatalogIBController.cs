using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Extensions;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Security;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using Nop.Core.Infrastructure;


namespace Nop.Web.Controllers
{
    public partial class CatalogController
    {


        [ChildActionOnly]
        public ActionResult HomepageVenderProducts(int? productThumbPictureSize, CatalogPagingFilteringModel command)
        {

            //IList<int> filterableSpecificationAttributeOptionIds;
            //var products = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds, true,
            //    vendorId: vendor.Id,
            //    storeId: _storeContext.CurrentStore.Id,
            //    visibleIndividuallyOnly: true,
            //    orderBy: (ProductSortingEnum)command.OrderBy,
            //    pageIndex: command.PageNumber - 1,
            //    pageSize: command.PageSize);
            //var models = PrepareProductOverviewModels(products).ToList();

            var model = new List<VendorModel>();
            var vendors = _vendorService.GetAllVendors();

            foreach (var vendor in vendors.Where(v => v.ShowOnHomePage))
            {



                var vendorModel = new VendorModel
                {
                    Id = vendor.Id,
                    Name = vendor.GetLocalized(x => x.Name),
                    Description = vendor.GetLocalized(x => x.Description),
                    MetaKeywords = vendor.GetLocalized(x => x.MetaKeywords),
                    MetaDescription = vendor.GetLocalized(x => x.MetaDescription),
                    MetaTitle = vendor.GetLocalized(x => x.MetaTitle),
                    SeName = vendor.GetSeName(),
                    AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors
                };

                PrepareSortingOptions(vendorModel.PagingFilteringContext, command);
                //view mode
                PrepareViewModes(vendorModel.PagingFilteringContext, command);
                //page size
                PreparePageSizeOptions(vendorModel.PagingFilteringContext, command,
                    vendor.AllowCustomersToSelectPageSize,
                    vendor.PageSizeOptions,
                    vendor.PageSize);

                IList<int> filterableSpecificationAttributeOptionIds;
                var products = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds, true,
                    vendorId: vendor.Id,
                    storeId: _storeContext.CurrentStore.Id,
                    visibleIndividuallyOnly: true,
                    orderBy: (ProductSortingEnum)command.OrderBy,
                    pageIndex: command.PageNumber - 1,
                    pageSize: command.PageSize);
                vendorModel.Products = PrepareProductOverviewModels(products).ToList();



                model.Add(vendorModel);
            }

            return PartialView(model);



        }

        public ActionResult BoutiqueShops()
        {
            var vendors = _vendorService.GetAllVendors();
            var shops = new List<VendorModel>();
            foreach (var vendor in vendors)
            {
                var vendorModel = new VendorModel
                {
                    Id = vendor.Id,
                    Name = vendor.GetLocalized(x => x.Name),
                    Description = vendor.GetLocalized(x => x.Description),
                    MetaKeywords = vendor.GetLocalized(x => x.MetaKeywords),
                    MetaDescription = vendor.GetLocalized(x => x.MetaDescription),
                    MetaTitle = vendor.GetLocalized(x => x.MetaTitle),
                    SeName = vendor.GetSeName(),
                    AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors,
                    ImageUrl = _pictureService.GetPictureUrl(vendor.PictureId,200)
                };
                shops.Add(vendorModel);
            }

            return View(shops);
        }

        //public ActionResult ShopByCategory(FormCollection f, CatalogPagingFilteringModel command)
        //{

        //    var model = new SearchModel();
        //    string cacheKey = string.Format(ModelCacheEventConsumer.SEARCH_CATEGORIES_MODEL_KEY,
        //    _workContext.WorkingLanguage.Id,
        //    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
        //    _storeContext.CurrentStore.Id);
            
        //    var categories = _cacheManager.Get(cacheKey, () =>
        //    {
        //        var categoriesModel = new List<SearchModel.CategoryModel>();
        //        all categories
        //        var allCategories = _categoryService.GetAllCategories();
        //         top cats


        //        return allCategories;

        //    });

        //    model.AvailableCategories = (from c in categories
        //                                 where (c.ParentCategoryId == 0)
        //                                 select new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToList();


        //    model.SubCategories = (from c in categories
        //                           where (c.ParentCategoryId != 0)
        //                           select new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToList();
        //    foreach (var item in categories.Where(c => c.ParentCategoryId != 0))
        //    {
        //        model.SubCategories.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
        //    }

        //    var vendors = _vendorService.GetAllVendors();

        //    model.Vendors = (from v in vendors
        //                         select new SelectListItem { Text = v.Name, Value = v.Id.ToString() }).ToList();


        //    CustomDataKeyGroupNames
        //    var styles = _customDataService.GetCustomDataByKeyGroup(CustomDataKeyGroupNames.Style);
        //    var circaDates = _customDataService.GetCustomDataByKeyGroup(CustomDataKeyGroupNames.CircaDate);
        //    model.CustomData = ((from st in styles
        //                         select new SelectListItem { Text = st.Value, Value = st.Key })
        //                        .Union
        //                            (from cd in circaDates
        //                             select new SelectListItem { Text = cd.Value, Value = cd.Key })).ToList();

        //    model.Dimension = _measureService.GetAllMeasureDimensions().Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();

        //    return View(model);
        
        //}

        //[HttpPost]
        public ActionResult ShopByCategory(FormCollection f, CatalogPagingFilteringModel command)
        {

            var model = new SearchModel();
            string cacheKey = string.Format(ModelCacheEventConsumer.SEARCH_CATEGORIES_MODEL_KEY,
            _workContext.WorkingLanguage.Id,
            string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
            _storeContext.CurrentStore.Id);


            var categoryIds = new List<int>();
            var mainCatIds = string.IsNullOrWhiteSpace(f["CategoryList"]) ? new string[] { } : f["CategoryList"].Split(new char[] { ',' });
            var subCats = string.IsNullOrWhiteSpace(f["SubCatList"]) ? new string[] { } : f["SubCatList"].Split(new char[] { ',' });
            var pvendorIds = string.IsNullOrWhiteSpace(f["VendorList"]) ? new List<int>() : f["VendorList"].Split(new char[] { ',' }).Select(c => int.Parse(c)).ToList();
            var customKeys = string.IsNullOrWhiteSpace(f["CustomList"]) ? new List<string>() { } : f["CustomList"].Split(new char[] { ',' }).ToList();
            categoryIds.AddRange(mainCatIds.Select(c => int.Parse(c)).ToList());
            categoryIds.AddRange(subCats.Select(c => int.Parse(c)).ToList());
            var mdId = 0;
            
            int.TryParse(f["SelectedDimension"],out mdId);
            var dm = _measureService.GetMeasureDimensionById(mdId);
            decimal dsizeFrm = 0;
            decimal dsizeTo = 0;
            decimal.TryParse(f["SizeFrom"], out dsizeFrm);
            decimal.TryParse(f["SizeTo"], out dsizeTo);

            PrepareViewModes(model.PagingFilteringContext, command);

            PreparePageSizeOptions(model.PagingFilteringContext, command,
               _catalogSettings.SearchPageAllowCustomersToSelectPageSize,
               _catalogSettings.SearchPagePageSizeOptions,
               _catalogSettings.SearchPageProductsPerPage);


            var categories = _cacheManager.Get(cacheKey, () =>
            {
                var categoriesModel = new List<SearchModel.CategoryModel>();
                //all categories
                var allCategories = _categoryService.GetAllCategories();
                // top cats


                return allCategories;

            });

            model.AvailableCategories = (from c in categories
                                         where (c.ParentCategoryId == 0)
                                         select new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = categoryIds.Contains(c.Id) }).ToList();


            model.SubCategories = (from c in categories
                                   where (c.ParentCategoryId != 0)
                                   select new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = categoryIds.Contains(c.Id) }).ToList();
            //foreach (var item in categories.Where(c => c.ParentCategoryId != 0))
            //{
            //    model.SubCategories.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            //}

            var vendors = _vendorService.GetAllVendors();

            model.Vendors = (from v in vendors
                             select new SelectListItem { Text = v.Name, Value = v.Id.ToString(), Selected = pvendorIds.Contains(v.Id) }).ToList();


            //CustomDataKeyGroupNames
            var styles = _customDataService.GetCustomDataByKeyGroup(CustomDataKeyGroupNames.Style);
            var circaDates = _customDataService.GetCustomDataByKeyGroup(CustomDataKeyGroupNames.CircaDate);
            model.CustomData = ((from st in styles
                                 select new SelectListItem { Text = st.Value, Value = st.Key, Selected = customKeys.Contains(st.Key) })
                                .Union
                                    (from cd in circaDates
                                     select new SelectListItem { Text = cd.Value, Value = cd.Key, Selected = customKeys.Contains(cd.Key) })).ToList();

            model.Dimension = _measureService.GetAllMeasureDimensions().Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();
            model.SelectedDimension = mdId;
            model.SizeFrom = dsizeFrm;
            model.SizeTo = dsizeTo;

            decimal sf = 0;
            decimal sst = 0;
            if(dm!=null)
            {
                sf = _measureService.ConvertFromPrimaryMeasureDimension(dsizeFrm, dm);
                sst = _measureService.ConvertFromPrimaryMeasureDimension(dsizeTo, dm);
            }
                
          
           
                
            var ipageSize = EngineContext.Current.Resolve<Nop.Core.Domain.Common.AdminAreaSettings>().DefaultGridPageSize;
            if (command.PageSize == 0)
                command.PageSize = ipageSize;
            if (command.PageNumber == 0)
                command.PageNumber = 1;
            var products = _productService.SearchProductsCustom(
                     categoryIds: categoryIds,
                     vendorIds: pvendorIds,
                     customKeys: customKeys,
                     sizeFrom: sf,
                     sizeTo: sst,
                     storeId: _storeContext.CurrentStore.Id,
                     visibleIndividuallyOnly: true,
                     keywords: f["q"],
                     languageId: _workContext.WorkingLanguage.Id,
                     orderBy: (ProductSortingEnum)command.OrderBy,
                     pageIndex: command.PageNumber - 1,
                     pageSize: command.PageSize);


            model.Products = PrepareProductOverviewModels(products).OrderBy(p=>p.Name).ToList();
            model.PagingFilteringContext.LoadPagedList(products);

            return View(model);
        }

        //[HttpPost]
        public ActionResult GetSubCategory(List<int> ids)
        {

            var allCategories = new List<Category>();
            if (ids ==null)
            {
                allCategories = _categoryService.GetAllCategories().Where(c=>c.ParentCategoryId!=0).ToList();
               
            }else
            {
                foreach (int id in ids)
                {
                    allCategories.AddRange(_categoryService.GetAllCategoriesByParentCategoryId(id));
                }
            }
           



            var data = allCategories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

            return PartialView("_SubCatList", data);
            //return Json (data,JsonRequestBehavior.AllowGet);
           
            
        }

        [HttpPost]
        public ActionResult ShopByCategoryPaged(FormCollection f, CatalogPagingFilteringModel command)
        {

            var model = new SearchModel();
            string cacheKey = string.Format(ModelCacheEventConsumer.SEARCH_CATEGORIES_MODEL_KEY,
            _workContext.WorkingLanguage.Id,
            string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
            _storeContext.CurrentStore.Id);


            var categoryIds = new List<int>();
            var mainCatIds = string.IsNullOrWhiteSpace(f["CategoryList"]) ? new string[] { } : f["CategoryList"].Split(new char[] { ',' });
            var subCats = string.IsNullOrWhiteSpace(f["SubCatList"]) ? new string[] { } : f["SubCatList"].Split(new char[] { ',' });
            var pvendorIds = string.IsNullOrWhiteSpace(f["VendorList"]) ? new List<int>() : f["VendorList"].Split(new char[] { ',' }).Select(c => int.Parse(c)).ToList();
            var customKeys = string.IsNullOrWhiteSpace(f["CustomList"]) ? new List<string>() { } : f["CustomList"].Split(new char[] { ',' }).ToList();
            categoryIds.AddRange(mainCatIds.Select(c => int.Parse(c)).ToList());
            categoryIds.AddRange(subCats.Select(c => int.Parse(c)).ToList());
            var mdId = 0;

            int.TryParse(f["SelectedDimension"], out mdId);
            var dm = _measureService.GetMeasureDimensionById(mdId);
            decimal dsizeFrm = 0;
            decimal dsizeTo = 0;
            decimal.TryParse(f["SizeFrom"], out dsizeFrm);
            decimal.TryParse(f["SizeTo"], out dsizeTo);

            PrepareViewModes(model.PagingFilteringContext, command);

            PreparePageSizeOptions(model.PagingFilteringContext, command,
               _catalogSettings.SearchPageAllowCustomersToSelectPageSize,
               _catalogSettings.SearchPagePageSizeOptions,
               _catalogSettings.SearchPageProductsPerPage);


            var categories = _cacheManager.Get(cacheKey, () =>
            {
                var categoriesModel = new List<SearchModel.CategoryModel>();
                //all categories
                var allCategories = _categoryService.GetAllCategories();
                // top cats


                return allCategories;

            });

            model.AvailableCategories = (from c in categories
                                         where (c.ParentCategoryId == 0)
                                         select new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = categoryIds.Contains(c.Id) }).ToList();


            model.SubCategories = (from c in categories
                                   where (c.ParentCategoryId != 0)
                                   select new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = categoryIds.Contains(c.Id) }).ToList();
            //foreach (var item in categories.Where(c => c.ParentCategoryId != 0))
            //{
            //    model.SubCategories.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            //}

            var vendors = _vendorService.GetAllVendors();

            model.Vendors = (from v in vendors
                             select new SelectListItem { Text = v.Name, Value = v.Id.ToString(), Selected = pvendorIds.Contains(v.Id) }).ToList();


            //CustomDataKeyGroupNames
            var styles = _customDataService.GetCustomDataByKeyGroup(CustomDataKeyGroupNames.Style);
            var circaDates = _customDataService.GetCustomDataByKeyGroup(CustomDataKeyGroupNames.CircaDate);
            model.CustomData = ((from st in styles
                                 select new SelectListItem { Text = st.Value, Value = st.Key, Selected = customKeys.Contains(st.Key) })
                                .Union
                                    (from cd in circaDates
                                     select new SelectListItem { Text = cd.Value, Value = cd.Key, Selected = customKeys.Contains(cd.Key) })).ToList();

            model.Dimension = _measureService.GetAllMeasureDimensions().Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();
            model.SelectedDimension = mdId;
            model.SizeFrom = dsizeFrm;
            model.SizeTo = dsizeTo;

            decimal sf = 0;
            decimal sst = 0;
            if (dm != null)
            {
                sf = _measureService.ConvertFromPrimaryMeasureDimension(dsizeFrm, dm);
                sst = _measureService.ConvertFromPrimaryMeasureDimension(dsizeTo, dm);
            }



            var pgN = int.Parse(string.IsNullOrWhiteSpace(f["PageNo"]) ? "0" : f["PageNo"]);
            var ipageSize = EngineContext.Current.Resolve<Nop.Core.Domain.Common.AdminAreaSettings>().DefaultGridPageSize;
            if (command.PageSize == 0)
                command.PageSize = ipageSize;
            if (pgN == 0)
                command.PageNumber = 1;
            else command.PageNumber = pgN;

            var products = _productService.SearchProductsCustom(
                     categoryIds: categoryIds,
                     vendorIds: pvendorIds,
                     customKeys: customKeys,
                     sizeFrom: sf,
                     sizeTo: sst,
                     storeId: _storeContext.CurrentStore.Id,
                     visibleIndividuallyOnly: true,
                     keywords: f["q"],
                     languageId: _workContext.WorkingLanguage.Id,
                     orderBy: (ProductSortingEnum)command.OrderBy,
                     pageIndex: command.PageNumber - 1,
                     pageSize: command.PageSize);


            model.Products = PrepareProductOverviewModels(products).OrderBy(p => p.Name).ToList();
            model.PagingFilteringContext.LoadPagedList(products);

            return PartialView("_ProductList",model.Products);
        }

        public ActionResult BoutiqueShopDetails(int vendorId)
        {
            // var products = _productService.GetAllProductsForVendorId(vendorId);
            //var productOverviewModel= PrepareProductOverviewModels(products).ToList();
            //var shops = new List<VendorModel>();
            //foreach (var vendor in vendors)
            //{
            //    var vendorModel = new VendorModel
            //    {
            //        Id = vendor.Id,
            //        Name = vendor.GetLocalized(x => x.Name),
            //        Description = vendor.GetLocalized(x => x.Description),
            //        MetaKeywords = vendor.GetLocalized(x => x.MetaKeywords),
            //        MetaDescription = vendor.GetLocalized(x => x.MetaDescription),
            //        MetaTitle = vendor.GetLocalized(x => x.MetaTitle),
            //        SeName = vendor.GetSeName(),
            //        AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors,
            //        ImageUrl = _pictureService.GetPictureUrl(vendor.PictureId, 200)
            //    };
            //    shops.Add(vendorModel);
            //}
            ProductOverviewModel model = new ProductOverviewModel();
            model.Id = vendorId;
            var vendor = _vendorService.GetVendorById(vendorId);
            var vendorModel = new VendorModel
            {
                Id = vendor.Id,
                ImageUrl = _pictureService.GetPictureUrl(vendor.PictureId,200),
                Name = vendor.Name,
                SeName = vendor.GetSeName(),
                Description = vendor.Description,
                Coutry = vendor.Country,
                City = vendor.City,
                Web = vendor.Web
            };


            model.Vendor = vendorModel;
            return View(model);
        }

        [HttpPost, AdminAntiForgeryAttribute(true)]
        public ActionResult GetProductListVendorId(int vendorId, string orderById, string searchName)
        {
            var products = _productService.GetAllProductsForVendorId(vendorId, orderById, searchName);


            var productOverviewModel = PrepareProductOverviewModels(products).ToList();

            var productDetail = this.RenderPartialViewToString("_VendorProducts", productOverviewModel);

            return Json(
                new
                {
                    success = true,
                    // JsonRequestBehavior.AllowGet,
                    message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart")),
                    productListHtml = productDetail,
                });
        }
    }
}