using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Vendors;
using Nop.Web.Extensions;
using Nop.Web.Models.Catalog;
using Nop.Core;
using Nop.Services.Messages;
using Nop.Core.Domain.Messages;

namespace Nop.Web.Controllers
{
    public partial class ProductController
    {
        IVendorService _venderService;
        INewsLetterSubscriptionService _newsLetterSubscriptionService;


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
            var products = string.IsNullOrEmpty(q) ? _productService.GetLatestProductsDisplayedOnHomePage() :
                _productService.GetLatestProducts(product => product.Name.ToLower().Contains(q.ToLower()));
            
            if (s == "0")
                products = products.OrderByDescending(p => p.CreatedOnUtc).ToList();
            else if (s == "1")
                products = products.OrderBy(p => p.Name).ToList();


            ViewBag.PageCount = (products.Count % pageSize) > 1 ? (1 + (products.Count / pageSize)) : Convert.ToInt32((products.Count / pageSize));

            

            products = products.Skip(skip).Take(pageSize).ToList();
           
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            //if (products.Count() == 0)
            //    return Content("");

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
            if (model.Subscribe)
            {

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

         [NonAction]
        public bool SubscribeNewsletter(string email, bool subscribe)
        {
            string result;
            bool success = false;

            if (!CommonHelper.IsValidEmail(email))
            {
                result = _localizationService.GetResource("Newsletter.Email.Wrong");
            }
            else
            {
                email = email.Trim();

                var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(email, _storeContext.CurrentStore.Id);
                if (subscription != null)
                {
                    if (subscribe)
                    {
                        if (!subscription.Active)
                        {
                            _workflowMessageService.SendNewsLetterSubscriptionActivationMessage(subscription, _workContext.WorkingLanguage.Id);
                        }
                        result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                    }
                    else
                    {
                        if (subscription.Active)
                        {
                            _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessage(subscription, _workContext.WorkingLanguage.Id);
                        }
                        result = _localizationService.GetResource("Newsletter.UnsubscribeEmailSent");
                    }
                }
                else if (subscribe)
                {
                    subscription = new NewsLetterSubscription
                    {
                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                        Email = email,
                        Active = false,
                        StoreId = _storeContext.CurrentStore.Id,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                    _workflowMessageService.SendNewsLetterSubscriptionActivationMessage(subscription, _workContext.WorkingLanguage.Id);

                    result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                }
                else
                {
                    result = _localizationService.GetResource("Newsletter.UnsubscribeEmailSent");
                }
                success = true;
            }

            return success;
        }

       

    }
}