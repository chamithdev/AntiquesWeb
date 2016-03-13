﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Web.Controllers
{
    public partial class ProductController
    {
        IVendorService _venderService;



        [ChildActionOnly]
        public ActionResult HomepageLatestProducts(int id, int? productThumbPictureSize)
        {
            var products = _productService.GetLatestProductsDisplayedOnHomePage();
           
            products = products.OrderBy(p => Guid.NewGuid()).ToList();
            if (id==1)
                products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            else
                products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).Take(20).ToList();
           
            products = products.Where(p => p.IsAvailable()).ToList();

            if (products.Count == 0)
                return Content("");

            var model = PrepareProductOverviewModelsIB(products, true, true, productThumbPictureSize).ToList();
            model = model.OrderBy(p => Guid.NewGuid()).ToList();
            return PartialView(model);
        }

        //[ChildActionOnly]
        public ActionResult LatestFinds(int? pageNo, string q = "", string s = "")
        {
            var pageSize = _catalogSettings.SearchPageProductsPerPage;


            if (s == "")
                s = "0";
            if (string.IsNullOrWhiteSpace(q))
                q = "";

            if (pageNo == null)
                pageNo = 1;

            int skip = (pageNo.Value -1) * pageSize;
            var products = _productService.GetLatestProductsDisplayedOnHomePage();

            products = products.Where(p => p.Name.Contains(q) || q == "").ToList();
            if (s == "0")
                products = products.OrderByDescending(p => p.CreatedOnUtc).ToList();
            else if (s == "1")
                products = products.OrderBy(p => p.Name).ToList();


            ViewBag.PageCount = (products.Count % pageSize) > 1 ? (1 + (products.Count / pageSize)) : Convert.ToInt32((products.Count / pageSize));

            

            products = products.Skip(skip).Take(pageSize).ToList();
           
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (products.Count() == 0)
                return Content("");

            var model = PrepareProductOverviewModelsIB(products, true, true).ToList();
            if (s == "0")
                model = model.OrderBy(p => p.CreateDateUtc).ToList();
            else if (s == "1")
                model = model.OrderBy(p => p.Name).ToList();
           
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult ProductInquire(ProductInquieryModel model)
        {
            var product = _productService.GetProductById(model.Id);
            var vendor = _venderService.GetVendorById(product.VendorId);
            if (product != null && vendor != null)
            {
                _workflowMessageService.SendProductInquery(product,vendor,model.Name,model.Phone,model.Email,model.Message, _workContext.WorkingLanguage.Id);
            }

            return Json(new {Success=true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult VendorInquire(ProductInquieryModel model)
        {
           
            var vendor = _venderService.GetVendorById(model.VendorId);
            if (vendor != null)
            {
                _workflowMessageService.SendVendorInquery(vendor, model.Name, model.Phone, model.Email, model.Message, _workContext.WorkingLanguage.Id);
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
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

       

    }
}