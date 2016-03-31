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
                vendorModel.Products = PrepareProductOverviewModelsIB(products).ToList();
               

                model.Add(vendorModel);
            }

            return PartialView(model);



        }


        [NonAction]
        protected virtual IEnumerable<ProductOverviewModel> PrepareProductOverviewModelsIB(IEnumerable<Product> products,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false)
        {
            return this.PrepareProductOverviewModels(_workContext,
                _storeContext, _categoryService, _productService, _specificationAttributeService,
                _priceCalculationService, _priceFormatter, _permissionService,
                _localizationService, _taxService, _currencyService,
                _pictureService,_customDataService, _webHelper, _cacheManager,
                _catalogSettings, _mediaSettings, products,
                preparePriceModel, preparePictureModel,
                productThumbPictureSize, prepareSpecificationAttributes,
                forceRedirectionAfterAddingToCart);
        }

        public ActionResult BoutiqueShops(int?pageNo,string q="",string s="")
        {
            //var q="";
         
            //if (f["q"] != null)
            //    q = f["q"].ToString();
            if (s == "")
                s="0";
            if (string.IsNullOrWhiteSpace(q)) 
                q = "";

            List<Vendor> vendors = _vendorService.GetAllVendors(name: q).ToList();
            if (s == "0")
                vendors = _vendorService.GetAllVendorsOrderByDate(name: q).ToList();
            else if(s=="1")
                vendors = vendors.OrderBy(v => v.Name).ToList();
                        
            var pageSize = _catalogSettings.SearchPageProductsPerPage;
            
            if (pageNo == null)
                pageNo = 1;

            int skip = (pageNo.Value - 1) * pageSize;

            ViewBag.PageCount = (vendors.Count() % pageSize) > 1 ? (1 + (vendors.Count() / pageSize)) : Convert.ToInt32((vendors.Count() / pageSize));

            var lstvendors = vendors.Skip(skip).Take(pageSize).ToList();
            var shops = new List<VendorModel>();
            foreach (var vendor in lstvendors)
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
                    ImageUrl = _pictureService.GetPictureUrl(vendor.PictureId)
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

            var model =  GetCatalogSearhModel(f, command);//new SearchModel();
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
            

            var categoriesModel = new List<SearchModel.CategoryModel>();
            //all categories
            var categories = _categoryService.GetAllCategories();
            // top cats


            //return allCategories;


            model.AvailableCategories = (from c in categories
                                         where (c.ParentCategoryId == 0)
                                         select new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = categoryIds.Contains(c.Id) }).ToList();


            if (mainCatIds.Count() == 0)
                model.SubCategories = new List<SelectListItem>();
            else
            {
                model.SubCategories = (from c in categories
                                       where (mainCatIds.Contains(c.ParentCategoryId.ToString()))
                                       select new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = categoryIds.Contains(c.Id) }).ToList();
            }
            
         

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
            model.q = f["q"];
            

            return View(model);
        }

        //[HttpPost]
        public ActionResult GetSubCategory(List<int> ids, List<int> selids)
        {
            if (selids == null)
                selids = new List<int>();
            var allCategories = new List<Category>();
            if (ids ==null)
            {
                allCategories = new List<Category>(); //_categoryService.GetAllCategories().Where(c => c.ParentCategoryId != 0).ToList();
               
            }else
            {
                foreach (int id in ids)
                {
                    allCategories.AddRange(_categoryService.GetAllCategoriesByParentCategoryId(id));
                }
            }




            var data = allCategories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name, Selected = selids.Contains(c.Id) }).ToList();

            return PartialView("_SubCatList", data);
            //return Json (data,JsonRequestBehavior.AllowGet);
           
            
        }

        [HttpPost]
        public ActionResult ShopByCategoryPaged(FormCollection f, CatalogPagingFilteringModel command)
        {
            var model = GetCatalogSearhModel(f, command);
           
            return PartialView("_ProductList",model.Products);
        }

        private SearchModel GetCatalogSearhModel(FormCollection f, CatalogPagingFilteringModel command)
        {
             var model = new SearchModel();
            //string cacheKey = string.Format(ModelCacheEventConsumer.SEARCH_CATEGORIES_MODEL_KEY,
            //_workContext.WorkingLanguage.Id,
            //string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
            //_storeContext.CurrentStore.Id);


            var categoryIds = new List<int>();
            var mainCatIds = string.IsNullOrWhiteSpace(f["CategoryList"]) ? new string[] { } : f["CategoryList"].Split(new char[] { ',' });
            var subCats = string.IsNullOrWhiteSpace(f["SubCatList"]) ? new string[] { } : f["SubCatList"].Split(new char[] { ',' });
            var pvendorIds = string.IsNullOrWhiteSpace(f["VendorList"]) ? new List<int>() : f["VendorList"].Split(new char[] { ',' }).Select(c => int.Parse(c)).ToList();
            var customKeys = string.IsNullOrWhiteSpace(f["CustomList"]) ? new List<string>() { } : f["CustomList"].Split(new char[] { ',' }).ToList();
            categoryIds.AddRange(mainCatIds.Select(c => int.Parse(c)).ToList());
            if (subCats.Length>0)
                categoryIds.AddRange(subCats.Select(c => int.Parse(c)).ToList());
            else
            {
                foreach(string mid in mainCatIds)
                {
                    var subcatids = _categoryService.GetAllCategoriesByParentCategoryId(Convert.ToInt32(mid)).Select(c => c.Id).ToArray();
                    categoryIds.AddRange(subcatids);
                }
            }
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


            model.SelectedDimension = mdId;
            model.SizeFrom = dsizeFrm;
            model.SizeTo = dsizeTo;

            decimal sf = 0;
            decimal sst = 0;
            decimal varience = 0;
            var cmdm = _measureService.GetMeasureDimensionBySystemKeyword("centimeters");
            if (dm != null)
            {
                sf = _measureService.ConvertFromPrimaryMeasureDimension(dsizeFrm, dm);
                sst = _measureService.ConvertFromPrimaryMeasureDimension(dsizeTo, dm);
                varience = _measureService.ConvertDimension(10, cmdm, dm);
            }



            var pgN = int.Parse(string.IsNullOrWhiteSpace(f["PageNo"]) ? "0" : f["PageNo"]);
            var ipageSize = _catalogSettings.SearchPageProductsPerPage;
            if (command.PageSize == 0)
                command.PageSize = ipageSize;
            if (pgN == 0)
                command.PageNumber = 1;
            else command.PageNumber = pgN;
            if(f.AllKeys.Count()!=0)
            {
                var products = _productService.SearchProductsCustom(
                     categoryIds: categoryIds,
                     vendorIds: pvendorIds,
                     customKeys: customKeys,
                     sizeFrom: sf,
                     sizeTo: sst,
                     varience: varience,
                     storeId: _storeContext.CurrentStore.Id,
                     visibleIndividuallyOnly: true,
                     keywords: f["q"],
                     languageId: _workContext.WorkingLanguage.Id,
                     orderBy: (ProductSortingEnum)command.OrderBy,
                     pageIndex: command.PageNumber - 1,
                     pageSize: command.PageSize);


                model.Products = PrepareProductOverviewModelsIB(products).OrderBy(p => p.Name).ToList();
                model.PagingFilteringContext.LoadPagedList(products);

            }
            
            return model;
        }

        public ActionResult BoutiqueShopDetails(int vendorId, string s, string q, int? pageNo)
        {
           
            if (s == "")
                s = "0";
            if (string.IsNullOrWhiteSpace(q))
                q = "";

            ProductOverviewModel model = new ProductOverviewModel();
            var pageSize = _catalogSettings.SearchPageProductsPerPage;
            if (pageNo == null)
                pageNo = 1;

            int skip = (pageNo.Value - 1) * pageSize;

            model.Id = vendorId;
            var vendor = _vendorService.GetVendorById(vendorId);
            var products = _productService.GetAllProductsForVendorId(vendorId, s, q);
            ViewBag.PageCount = (products.Count() % pageSize) > 1 ? (1 + (products.Count() / pageSize)) : Convert.ToInt32((products.Count() / pageSize));

            products = products.Skip(skip).Take(pageSize).ToList();

            var productOverviewModel = PrepareProductOverviewModelsIB(products).ToList();

            var firstCustomer = _customerService
                .GetAllCustomers(vendorId: vendor.Id)
                .OrderBy(v => v.CreatedOnUtc)
                .FirstOrDefault();

            var phone = string.Empty;
            if (firstCustomer != null)
            {
                var attribs = _genericAttributeService.GetAttributesForEntity(firstCustomer.Id, "Customer");
                var phoneAttribute = attribs.FirstOrDefault(a => a.Key == SystemCustomerAttributeNames.Phone);
                if (phoneAttribute != null)
                {
                    phone = phoneAttribute.Value;
                }
            }
            
            var vendorModel = new VendorModel
            {
                Id = vendor.Id,
                ImageUrl = _pictureService.GetPictureUrl(vendor.PictureId),
                Name = vendor.Name,
                SeName = vendor.GetSeName(),
                Description = vendor.Description,
                Coutry = vendor.Country,
                City = vendor.City,
                Web = vendor.Web,
                Products = productOverviewModel,
                PhoneOfTheFirstCustomer = phone
            };


            model.Vendor = vendorModel;
            
            return View(model);
        }

        [HttpPost, AdminAntiForgeryAttribute(true)]
        public ActionResult GetProductListVendorId(int vendorId, string orderById, string searchName, int? pageNo)
        {

            var pageSize = _catalogSettings.SearchPageProductsPerPage;
            if (pageNo == null)
                pageNo = 1;

            int skip = (pageNo.Value - 1) * pageSize;

            

            var products = _productService.GetAllProductsForVendorId(vendorId, orderById, searchName);
            ViewBag.PageCount = (products.Count() % pageSize) > 1 ? (1 + (products.Count() / pageSize)) : Convert.ToInt32((products.Count() / pageSize));

            products = products.Skip(skip).Take(pageSize).ToList();

            var productOverviewModel = PrepareProductOverviewModelsIB(products).ToList();
           
            //products
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