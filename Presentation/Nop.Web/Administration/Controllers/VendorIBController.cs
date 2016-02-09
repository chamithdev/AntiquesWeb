using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;
using System.IO;
using Nop.Admin.Models.Vendors;
using Nop.Admin.Extensions;
using Nop.Core.Infrastructure;
namespace Nop.Admin.Controllers
{
    public partial class VendorController
    {

        public ActionResult EditIB(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(id);
            if (vendor == null || vendor.Deleted)
                //No vendor found with the specified id
                return RedirectToAction("List");

            var model = vendor.ToModel();
            //pictuere
            model.MainPicture = _pictureService.GetPictureById(model.PictureId);
            if (model.MainPicture != null)
            {
                var m = new VendorModel.VenddorPictureModel
                {

                    ProductId = model.MainPicture.Id,
                    PictureUrl = _pictureService.GetPictureUrl(model.MainPicture),
                    OverrideAltAttribute = model.MainPicture.AltAttribute,
                    OverrideTitleAttribute = model.MainPicture.TitleAttribute,
                    DisplayOrder = 1
                };
                model.MainPictureModel = m;
            }

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = vendor.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = vendor.GetLocalized(x => x.Description, languageId, false, false);
                locale.MetaKeywords = vendor.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = vendor.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = vendor.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = vendor.GetSeName(languageId, false, false);
            });
            //associated customer emails
            model.AssociatedCustomerEmails = _customerService
                    .GetAllCustomers(vendorId: vendor.Id)
                    .Select(c => c.Email)
                    .ToList();

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing"), AdminAntiForgeryAttribute(true)]
        public ActionResult EditIB(VendorModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(model.Id);
            if (vendor == null || vendor.Deleted)
                //No vendor found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                vendor = model.ToEntity(vendor);
                _vendorService.UpdateVendor(vendor);
                //search engine name
                model.SeName = vendor.ValidateSeName(model.SeName, vendor.Name, true);
                _urlRecordService.SaveSlug(vendor, model.SeName, 0);
                //locales
                UpdateLocales(vendor, model);

                SuccessNotification(_localizationService.GetResource("Admin.Vendors.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabIndex();

                    return RedirectToAction("Edit", new { id = vendor.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //associated customer emails
            model.AssociatedCustomerEmails = _customerService
                    .GetAllCustomers(vendorId: vendor.Id)
                    .Select(c => c.Email)
                    .ToList();
            //return View(model);
            return RedirectToAction("EditIB", new { id = vendor.Id });
        }


         [HttpPost, AdminAntiForgeryAttribute(true)]
        public ActionResult UploadVendorImages(string id)
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            try
            {
                var vendor = _vendorService.GetVendorById(int.Parse(id));
                if (vendor == null)
                    throw new ArgumentException("No vendor found with the specified id");
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    var pic = UploadPicture(file);

                    if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                        return AccessDeniedView();



                  

                    var picture = _pictureService.GetPictureById(pic.Id);
                    if (picture == null)
                        throw new ArgumentException("No picture found with the specified id");

                    vendor.PictureId = picture.Id;

                    _vendorService.UpdateVendor(vendor);

                    return RedirectToAction("Edit", new { id = vendor.Id });
                    

                   

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }


            if (isSavedSuccessfully)
            {
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }

        [NonAction]
        private Picture UploadPicture(HttpPostedFileBase postFile)
        {
            Stream stream = null;
            var fileName = "";
            var contentType = "";
           
            HttpPostedFileBase httpPostedFile = postFile;
            if (httpPostedFile == null)
                throw new ArgumentException("No file uploaded");
            stream = httpPostedFile.InputStream;
            fileName = Path.GetFileName(httpPostedFile.FileName);
            contentType = httpPostedFile.ContentType;


            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();
            //contentType is not always available 
            //that's why we manually update it here
            //http://www.sfsu.edu/training/mimetype.htm
            if (String.IsNullOrEmpty(contentType))
            {
                switch (fileExtension)
                {
                    case ".bmp":
                        contentType = "image/bmp";
                        break;
                    case ".gif":
                        contentType = "image/gif";
                        break;
                    case ".jpeg":
                    case ".jpg":
                    case ".jpe":
                    case ".jfif":
                    case ".pjpeg":
                    case ".pjp":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    case ".tiff":
                    case ".tif":
                        contentType = "image/tiff";
                        break;
                    default:
                        break;
                }
            }

            var picture = _pictureService.InsertPicture(fileBinary, contentType, null);
            return picture;
        }

        [NonAction]
        public List<VendorModel.VenddorProductModel> VendorProducts(int id,int pageIndex=0)
        {
           
            var defaultGridPageSize = EngineContext.Current.Resolve<Nop.Core.Domain.Common.AdminAreaSettings>().DefaultGridPageSize;
            var products = _productService.SearchProducts(
              vendorId: id,             
              showHidden: true,
              pageIndex: pageIndex,
              pageSize: defaultGridPageSize
             
          );
            var model = new List<VendorModel.VenddorProductModel> ();
            foreach (var p in products)
	        {
		        var vpm = new VendorModel.VenddorProductModel();
                vpm.Id = id;
                vpm.ProductId=p.Id;
                vpm.ProductName = p.Name;
                var vpic = new VendorModel.VenddorPictureModel();
                var pic = _pictureService.GetPicturesByProductId(p.Id, 1).FirstOrDefault();
                var imgUrl = _pictureService.GetPictureUrl(pic, 200);
                if(pic != null)
                    vpic.PictureId = pic.Id;
                vpic.PictureUrl = imgUrl;
                vpm.ProductPicture = vpic;
                vpm.DisplayOrder = p.DisplayOrder;
                vpm.TotalCount = products.TotalCount;
                model.Add(vpm);
	        }

            var items = model.OrderBy(p=>p.DisplayOrder).ToList();
            return items;

        }
        public ActionResult PagedProductList(VendorModel.ProductPagingModel model)
        {
            var prodModels = VendorProducts(model.VendorId, model.PageIndex);

            return View("_VendorProduct", prodModels);
            //return new JsonResult
            //{
            //    Data = prodModels
            //};

        }

        [NonAction]
        public List<VendorModel.VenddorProductModel> VendorProducts(IList<Product> productList)
        {

            var model = new List<VendorModel.VenddorProductModel>();
            foreach (var p in productList)
            {
                var vpm = new VendorModel.VenddorProductModel();
                //vpm.Id = id;
                vpm.ProductId = p.Id;
                vpm.ProductName = p.Name;
                var vpic = new VendorModel.VenddorPictureModel();
                var pic = _pictureService.GetPicturesByProductId(p.Id, 1).FirstOrDefault();
                var imgUrl = _pictureService.GetPictureUrl(pic, 200);
                if (pic!= null)
                    vpic.PictureId = pic.Id;
                vpic.PictureUrl = imgUrl;
                vpm.ProductPicture = vpic;
                vpm.DisplayOrder = p.DisplayOrder;
               // vpm.TotalCount = products.TotalCount;
                model.Add(vpm);
            }

            var items = model;
            return items;

        }
        // 


        [HttpPost, AdminAntiForgeryAttribute(true)]
        public ActionResult UpdateProductOrder(List<VendorModel.VenddorProductModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var defaultGridPageSize = EngineContext.Current.Resolve<Nop.Core.Domain.Common.AdminAreaSettings>().DefaultGridPageSize;
            //defaultGridPageSize = 4;
            foreach (VendorModel.VenddorProductModel model in models)
            {
                var product = _productService.GetProductById(model.Id);
                if (product == null)
                    throw new ArgumentException("No product picture found with the specified id");

                product.DisplayOrder = (product.DisplayOrder - model.DisplayOrder );
                _productService.UpdateProduct(product);

                
            }
            

            return new NullJsonResult();
        }



        #region Vendor - Product Page
        public ActionResult UpdateDirectoryib(int vendorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var products = _productService.GetAllProductsForVendorId(vendorId);

            //if (products.Count == 0)
            //    return Content("");
            ViewBag.VendorId = vendorId;
            var model = VendorProducts(products);
            return View(model);
        }
        #endregion


        public ActionResult MyHome(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(id);
            if (vendor == null || vendor.Deleted)
                //No vendor found with the specified id
                return RedirectToAction("List");

            var model = vendor.ToModel();
            //pictuere
            model.MainPicture = _pictureService.GetPictureById(model.PictureId);
            if (model.MainPicture != null)
            {
                var m = new VendorModel.VenddorPictureModel
                {

                    ProductId = model.MainPicture.Id,
                    PictureUrl = _pictureService.GetPictureUrl(model.MainPicture, 200),
                    OverrideAltAttribute = model.MainPicture.AltAttribute,
                    OverrideTitleAttribute = model.MainPicture.TitleAttribute,
                    DisplayOrder = 1
                };
                model.MainPictureModel = m;
            }

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = vendor.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = vendor.GetLocalized(x => x.Description, languageId, false, false);
                locale.MetaKeywords = vendor.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = vendor.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = vendor.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = vendor.GetSeName(languageId, false, false);
            });
            //associated customer emails
            model.AssociatedCustomerEmails = _customerService
                    .GetAllCustomers(vendorId: vendor.Id)
                    .Select(c => c.Email)
                    .ToList();
            model.Products = VendorProducts(vendor.Id);
            model.DisplayActive = _workContext.IsAdmin;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing"), AdminAntiForgeryAttribute(true)]
        public ActionResult MyHome(VendorModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(model.Id);
            if (vendor == null || vendor.Deleted)
                //No vendor found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {

                vendor = model.ToEntity(vendor);

                _vendorService.UpdateVendor(vendor);

                if (vendor.Active && _workContext.IsAdmin)
                {
                    //send vendor email
                    var customer = _customerService.GetAllCustomers(vendorId: vendor.Id);
                    if (customer.Count > 0)
                    {
                        _workflowMessageService.SendVendorEmailValidationMessage(customer[0], _workContext.WorkingLanguage.Id);
                    }
                    //_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());

                }



                //search engine name
                model.SeName = vendor.ValidateSeName(model.SeName, vendor.Name, true);
                _urlRecordService.SaveSlug(vendor, model.SeName, 0);
                //locales
                UpdateLocales(vendor, model);

                SuccessNotification(_localizationService.GetResource("Admin.Vendors.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabIndex();

                    return RedirectToAction("Edit", new { id = vendor.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //associated customer emails
            model.AssociatedCustomerEmails = _customerService
                    .GetAllCustomers(vendorId: vendor.Id)
                    .Select(c => c.Email)
                    .ToList();
            //return View(model);
            return RedirectToAction("Edit", new { id = vendor.Id });
        }



    }
}