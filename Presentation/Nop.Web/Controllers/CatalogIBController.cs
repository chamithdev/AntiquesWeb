using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
                    ImageUrl = _pictureService.GetPictureUrl(vendor.PictureId),
                    ImageUrl2 = vendor.PictureId2 > 0 ? _pictureService.GetPictureUrl(vendor.PictureId2) : _pictureService.GetPictureUrl(vendor.PictureId)
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
        public ActionResult ShopByCategory()
        {

            var model = new SearchModel(); //GetCatalogSearhModel(f, command);//new SearchModel();
            string cacheKey = string.Format(ModelCacheEventConsumer.SEARCH_CATEGORIES_MODEL_KEY,
            _workContext.WorkingLanguage.Id,
            string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
            _storeContext.CurrentStore.Id);


            var categoryIds = new List<int>();
            var pvendorIds = new List<int>();
            var customKeys = new List<string>();
            

            var categoriesModel = new List<SearchModel.CategoryModel>();
            //all categories
            var categories = _categoryService.GetAllCategories();



            model.ProductCategories = categories.ToList();

            model.HeightMaxAvailable = _productService.MaxAvalableHeight();
            model.HeightMinAvailable = _productService.MinAvalableHeight();
            model.PriceMaxAvailable = _productService.MaxAvalablePrice();
            model.PriceMinAvailable = _productService.MinAvalablePrice();
            model.WidthMaxAvailable = _productService.MaxAvalableWidth();
            model.WidthMinAvailable = _productService.MinAvalableWidth();


            
         

            model.Styles = _customDataService.GetCustomDataByKeyGroup(CustomDataKeyGroupNames.Style).OrderBy(d => d.Value).ToList();

            model.Materials = _customDataService.GetCustomDataByKeyGroup(CustomDataKeyGroupNames.Material).OrderBy(d=>d.Value).ToList();

            model.Colors = _productService.GetColorList().Where(d => !string.IsNullOrWhiteSpace(d)).Select(d => new SelectListItem { Text = d, Value = d }).ToList();
            model.Colors.Insert(0, new SelectListItem { Text = "-", Value = "" });
            model.Designers = _productService.GetDesignerList().Where(d=> !string.IsNullOrWhiteSpace(d)).Select(d => new SelectListItem { Text = d, Value = d }).ToList();
            model.Designers.Insert(0, new SelectListItem { Text = "-", Value = "" });

            model.Dimension = _measureService.GetAllMeasureDimensions().Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();
            model.q = "";
            



            var products = _productService.SearchProductsCustom(
                    categoryIds: categoryIds,
                    vendorIds: pvendorIds,
                    customKeys: customKeys,
                    sizeFrom: 0,
                    sizeTo: 0,
                    varience: 0,
                    storeId: _storeContext.CurrentStore.Id,
                    visibleIndividuallyOnly: true,
                    keywords: "",
                    languageId: _workContext.WorkingLanguage.Id,
                    orderBy: ProductSortingEnum.Position,
                    pageIndex: 1,
                    pageSize: _catalogSettings.SearchPageProductsPerPage);


            model.Products = PrepareProductOverviewModelsIB(products).OrderBy(p => p.Name).ToList();
            model.PagingFilteringContext.LoadPagedList(products);
            

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
        public ActionResult ShopByCategoryPaged(SearchModel model, CatalogPagingFilteringModel command)
        {
            model = GetCatalogSearhModel(model, command);
           
            return PartialView("_ProductList",model);
        }

        private SearchModel GetCatalogSearhModel(SearchModel model, CatalogPagingFilteringModel command)
        {
             //var model = new SearchModel();
            //string cacheKey = string.Format(ModelCacheEventConsumer.SEARCH_CATEGORIES_MODEL_KEY,
            //_workContext.WorkingLanguage.Id,
            //string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
            //_storeContext.CurrentStore.Id);


            var categoryIds = new List<int>();

            var pvendorIds = new List<int>();
            var customKeys = model.ss ?? new List<string>();
            if (model.sms != null)
                customKeys.AddRange(model.sms);
            if (model.cid > 0)
                {
                categoryIds.Add(model.cid);
                var subcatids = _categoryService.GetAllCategoriesByParentCategoryId(model.cid).Select(c => c.Id).ToArray();
                    categoryIds.AddRange(subcatids);
                }
         

            var mdId = 0;

            
            decimal dsizeFrm = model.hm;
            decimal dsizeTo = model.hmx;
            

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
            decimal varience = 10;



            var pgN = model.pg;
            var ipageSize = _catalogSettings.SearchPageProductsPerPage;
            if (command.PageSize == 0)
                command.PageSize = ipageSize;
            if (pgN == 0)
                command.PageNumber = 1;
            else command.PageNumber = pgN;


                var products = _productService.SearchProductsCustom(
                     categoryIds: categoryIds,
                     vendorIds: pvendorIds,
                     customKeys: customKeys,
                   sizeFrom: model.hm,
                   sizeTo: model.hmx,
                     varience: varience,
                     storeId: _storeContext.CurrentStore.Id,
                     visibleIndividuallyOnly: true,
                   keywords:model.q,
                     languageId: _workContext.WorkingLanguage.Id,
                   orderBy: ProductSortingEnum.Position,
                     pageIndex: command.PageNumber - 1,
                   pageSize: command.PageSize,
                   circaDateFrom:model.cdf ?? "",
                   circaDateTo: model.cdt ?? "",
                   color:model.c ?? "",
                   designBy:model.d ?? "",
                   widthFrom:model.wm,
                   widthTo: model.wmx,
                   priceMax:model.pmx,
                   priceMin:model.pm);


                model.Products = PrepareProductOverviewModelsIB(products).OrderBy(p => p.Name).ToList();
                model.PagingFilteringContext.LoadPagedList(products);

            return model;
        }

        public ActionResult BoutiqueShopDetails(int vendorId, string s, string q, int? pageNo)
        {

            Expression<Func<Product, bool>> predicate = x => x.VendorId == vendorId && x.Published && x.ProductPictures.Count > 0;

            if (!string.IsNullOrWhiteSpace(q))
            {
                predicate = x => x.VendorId == vendorId && x.Published && x.Name.Equals(q);
            }
           
            IList<Product> products = new List<Product>();
            
            if (string.IsNullOrWhiteSpace(s) || s == "0")
            {
                products = _productService.GetAllProducts(predicate , x => x.StockQuantity == 0 ? 1 : 0 , x => x.DisplayOrder);
            }
            else if (s == "1")
            {
                products = _productService.GetAllProducts(predicate, x => x.CreatedOnUtc, false);
            }
            else if (s == "2")
            {
                products = _productService.GetAllProducts(predicate, x => x.Name);
            }
            

            ProductOverviewModel model = new ProductOverviewModel();
            var pageSize = _catalogSettings.SearchPageProductsPerPage;
            if (pageNo == null)
                pageNo = 1;

            int skip = (pageNo.Value - 1) * pageSize;

            model.Id = vendorId;

            var vendor = _vendorService.GetVendorById(vendorId);

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



           

    }
}