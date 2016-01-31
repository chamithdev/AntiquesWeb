﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Catalog;
using Nop.Admin.Models.Orders;
using Nop.Core;
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
namespace Nop.Admin.Controllers
{
    public partial class ProductController
    {
        #region Methods


        public ActionResult CreateIB()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new ProductModel();
            PrepareProductModel(model, null, true, true);
            AddLocales(_languageService, model.Locales);
            PrepareAclModel(model, null, false);
            PrepareStoresMappingModel(model, null, false);
            return View(model);
        }

        [HttpPost, AdminAntiForgeryAttribute(true)]
        public ActionResult CreateIB(ProductModel model)
        {
            bool continueEditing = true;
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            model.ProductTypeId = 5;
            model.ProductTemplateId = 1;
            model.Published = true;
            model.VisibleIndividually = true;
            if (ModelState.IsValid)
            {
                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null)
                {
                    model.VendorId = _workContext.CurrentVendor.Id;
                }
                //vendors cannot edit "Show on home page" property
                if (_workContext.CurrentVendor != null && model.ShowOnHomePage)
                {
                    model.ShowOnHomePage = false;
                }

                //product
                var product = model.ToEntity();
                product.CreatedOnUtc = DateTime.UtcNow;
                product.UpdatedOnUtc = DateTime.UtcNow;
                _productService.InsertProduct(product);
                //search engine name
                model.SeName = product.ValidateSeName(model.SeName, product.Name, true);
                _urlRecordService.SaveSlug(product, model.SeName, 0);
                //locales
                UpdateLocales(product, model);
                //ACL (customer roles)
                SaveProductAcl(product, model);
                //Stores
                SaveStoreMappings(product, model);
                //tags
                SaveProductTags(product, ParseProductTags(model.ProductTags));
                //warehouses
                SaveProductWarehouseInventory(product, model);
                //discounts
                var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToSkus, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        product.AppliedDiscounts.Add(discount);
                }
                _productService.UpdateProduct(product);
                _productService.UpdateHasDiscountsApplied(product);


                // attributes
                //var attributes = _productAttributeService.GetAllProductAttributes();
                //var attribute = attributes.FirstOrDefault(a => a.Name == "Circa Date");

                //if (attribute != null && model.CircaDate.Year != 1)
                //    SaveAttribute(model, attribute);

                //attribute = attributes.FirstOrDefault(a => a.Name == "Materials");
                //if (attribute != null &&!string.IsNullOrWhiteSpace(model.Material))
                //    SaveAttribute(model, attribute);

                //attribute = attributes.FirstOrDefault(a => a.Name == "Country");
                //if (attribute != null && !string.IsNullOrWhiteSpace(model.Country))
                //    SaveAttribute(model, attribute);

                //attribute = attributes.FirstOrDefault(a => a.Name == "DesignBy");
                //if (attribute != null && !string.IsNullOrWhiteSpace(model.DesignBy))
                //    SaveAttribute(model, attribute);

                //attribute = attributes.FirstOrDefault(a => a.Name == "TermsCondtion");
                //if (attribute != null )
                //    SaveAttribute(model, attribute);

                //attribute = attributes.FirstOrDefault(a => a.Name == "Color");
                //if (attribute != null && !string.IsNullOrWhiteSpace(model.Color))
                //    SaveAttribute(model, attribute);
                

                //activity log
                _customerActivityService.InsertActivity("AddNewProduct", _localizationService.GetResource("ActivityLog.AddNewProduct"), product.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = product.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareProductModel(model, null, false, true);
            PrepareAclModel(model, null, true);
            PrepareStoresMappingModel(model, null, true);
            return View(model);
        }
        [HttpPost,AdminAntiForgeryAttribute(true)]
        public ActionResult UploadProductImages(string id)
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    var pic = UploadPicture(file);

                    if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                        return AccessDeniedView();



                    var product = _productService.GetProductById(int.Parse(id));
                    if (product == null)
                        throw new ArgumentException("No product found with the specified id");

                    //a vendor should have access only to his products
                    if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                        return RedirectToAction("List");

                    var picture = _pictureService.GetPictureById(pic.Id);
                    if (picture == null)
                        throw new ArgumentException("No picture found with the specified id");

                    _productService.InsertProductPicture(new ProductPicture
                    {
                        PictureId = picture.Id,
                        ProductId = int.Parse(id),
                        DisplayOrder = 1,
                    });

                    _pictureService.UpdatePicture(picture.Id,
                        _pictureService.LoadPictureBinary(picture),
                        picture.MimeType,
                        picture.SeoFilename);

                    _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(product.Name));

                    //Save file content goes here
                    //fName = file.FileName;
                    //if (file != null && file.ContentLength > 0)
                    //{

                    //    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));

                    //    string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "imagepath");

                    //    var fileName1 = Path.GetFileName(file.FileName);

                    //    bool isExists = System.IO.Directory.Exists(pathString);

                    //    if (!isExists)
                    //        System.IO.Directory.CreateDirectory(pathString);

                    //    var path = string.Format("{0}\\{1}", pathString, file.FileName);
                    //    file.SaveAs(path);

                    //}

                    RedirectToAction("EditIB", new { id = id });

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



        public ActionResult EditIB(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(id);
            if (product == null || product.Deleted)
                //No product found with the specified id
                return RedirectToAction("List");
            
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            var model = product.ToModel();
            PrepareProductModel(model, product, false, false);
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = product.GetLocalized(x => x.Name, languageId, false, false);
                locale.ShortDescription = product.GetLocalized(x => x.ShortDescription, languageId, false, false);
                locale.FullDescription = product.GetLocalized(x => x.FullDescription, languageId, false, false);
                locale.MetaKeywords = product.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = product.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = product.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = product.GetSeName(languageId, false, false);
            });

            PrepareAclModel(model, product, false);
            PrepareStoresMappingModel(model, product, false);

            var productPictures = _productService.GetProductPicturesByProductId(product.Id);
            var productPicturesModel = productPictures
                .Select(x =>
                {
                    var picture = _pictureService.GetPictureById(x.PictureId);
                    if (picture == null)
                        throw new Exception("Picture cannot be loaded");
                    var m = new ProductModel.ProductPictureModel
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        PictureId = x.PictureId,
                        PictureUrl = _pictureService.GetPictureUrl(picture),
                        OverrideAltAttribute = picture.AltAttribute,
                        OverrideTitleAttribute = picture.TitleAttribute,
                        DisplayOrder = x.DisplayOrder
                    };
                    return m;
                })
                .ToList();
            model.ProductPictureModels = productPicturesModel;

            return View(model);
        }



        [HttpPost, AdminAntiForgeryAttribute(true)]
        public ActionResult EditIB(ProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            bool continueEditing = false;

            var product = _productService.GetProductById(model.Id);
            if (product == null || product.Deleted)
                //No product found with the specified id
                return RedirectToAction("List");

            model.ProductTypeId = 5;
            model.VisibleIndividually = true;
            model.ProductTemplateId = 1;
            model.Published = true;
       
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null)
                {
                    model.VendorId = _workContext.CurrentVendor.Id;
                }
                //vendors cannot edit "Show on home page" property
                if (_workContext.CurrentVendor != null && model.ShowOnHomePage != product.ShowOnHomePage)
                {
                    model.ShowOnHomePage = product.ShowOnHomePage;
                }
                var prevStockQuantity = product.GetTotalStockQuantity();

                //product
                product = model.ToEntity(product);
                product.UpdatedOnUtc = DateTime.UtcNow;
                _productService.UpdateProduct(product);
                //search engine name
                model.SeName = product.ValidateSeName(model.SeName, product.Name, true);
                _urlRecordService.SaveSlug(product, model.SeName, 0);
                //locales
                UpdateLocales(product, model);
                //tags
                SaveProductTags(product, ParseProductTags(model.ProductTags));
                //warehouses
                SaveProductWarehouseInventory(product, model);
                //ACL (customer roles)
                SaveProductAcl(product, model);
                //Stores
                SaveStoreMappings(product, model);
                //picture seo names
                UpdatePictureSeoNames(product);
                //discounts
                var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToSkus, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new discount
                        if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                            product.AppliedDiscounts.Add(discount);
                    }
                    else
                    {
                        //remove discount
                        if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                            product.AppliedDiscounts.Remove(discount);
                    }
                }
                _productService.UpdateProduct(product);
                _productService.UpdateHasDiscountsApplied(product);
                //back in stock notifications
                if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                    product.BackorderMode == BackorderMode.NoBackorders &&
                    product.AllowBackInStockSubscriptions &&
                    product.GetTotalStockQuantity() > 0 &&
                    prevStockQuantity <= 0 &&
                    product.Published &&
                    !product.Deleted)
                {
                    _backInStockSubscriptionService.SendNotificationsToSubscribers(product);
                }

                //activity log
                _customerActivityService.InsertActivity("EditProduct", _localizationService.GetResource("ActivityLog.EditProduct"), product.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabIndex();

                    return RedirectToAction("Edit", new { id = product.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareProductModel(model, product, false, true);
            PrepareAclModel(model, product, true);
            PrepareStoresMappingModel(model, product, true);
            return View(model);
        }



        [HttpPost, AdminAntiForgeryAttribute(true)]
        public ActionResult UpdateProductPicture(List<ProductModel.ProductPictureModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            foreach(ProductModel.ProductPictureModel model in models)
            {
                var productPicture = _productService.GetProductPictureById(model.Id);
                if (productPicture == null)
                    throw new ArgumentException("No product picture found with the specified id");

                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null)
                {
                    var product = _productService.GetProductById(productPicture.ProductId);
                    if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                    {
                        return Content("This is not your product");
                    }
                }

                productPicture.DisplayOrder = model.DisplayOrder;
                _productService.UpdateProductPicture(productPicture);

                var picture = _pictureService.GetPictureById(productPicture.PictureId);
                if (picture == null)
                    throw new ArgumentException("No picture found with the specified id");

                _pictureService.UpdatePicture(picture.Id,
                    _pictureService.LoadPictureBinary(picture),
                    picture.MimeType,
                    picture.SeoFilename,
                    model.OverrideAltAttribute,
                    model.OverrideTitleAttribute);
            }
            

            return new NullJsonResult();
        }






        [NonAction]
        private Picture UploadPicture(HttpPostedFileBase postFile)
        {
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            //if (String.IsNullOrEmpty(Request["qqfile"]))
            //{
            //    // IE
            //    HttpPostedFileBase httpPostedFile = Request.Files[0];
            //    if (httpPostedFile == null)
            //        throw new ArgumentException("No file uploaded");
            //    stream = httpPostedFile.InputStream;
            //    fileName = Path.GetFileName(httpPostedFile.FileName);
            //    contentType = httpPostedFile.ContentType;
            //}
            //else
            //{
            //    //Webkit, Mozilla
            //    stream = Request.InputStream;
            //    fileName = Request["qqfile"];
            //}

            // IE
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
        private void SaveAttribute(ProductModel model,ProductAttribute attr)
        {
            //var productAttributeMapping = new ProductAttributeMapping
            //{
            //    ProductId = model.Id,
            //    ProductAttributeId = attr.Id,
            //    TextPrompt = "",
            //    IsRequired = false,
            //    AttributeControlTypeId = 4,
            //    DisplayOrder = 1
            //};
            //_productAttributeService.InsertProductAttributeMapping(productAttributeMapping);

            //var pav = new ProductAttributeValue
            //    {
            //        ProductAttributeMappingId = productAttributeMapping.Id,
            //        AttributeValueType = AttributeValueType.Simple,
            //        Name = predefinedValue.Name,
            //        PriceAdjustment = predefinedValue.PriceAdjustment,
            //        WeightAdjustment = predefinedValue.WeightAdjustment,
            //        Cost = predefinedValue.Cost,
            //        IsPreSelected = predefinedValue.IsPreSelected,
            //        DisplayOrder = predefinedValue.DisplayOrder
            //    };
            //_productAttributeService.InsertProductAttributeValue(pav);

            ////predefined values
            //var predefinedValues = _productAttributeService.GetPredefinedProductAttributeValues(model.ProductAttributeId);
            //foreach (var predefinedValue in predefinedValues)
            //{
            //    var pav = new ProductAttributeValue
            //    {
            //        ProductAttributeMappingId = productAttributeMapping.Id,
            //        AttributeValueType = AttributeValueType.Simple,
            //        Name = predefinedValue.Name,
            //        PriceAdjustment = predefinedValue.PriceAdjustment,
            //        WeightAdjustment = predefinedValue.WeightAdjustment,
            //        Cost = predefinedValue.Cost,
            //        IsPreSelected = predefinedValue.IsPreSelected,
            //        DisplayOrder = predefinedValue.DisplayOrder
            //    };
            //    _productAttributeService.InsertProductAttributeValue(pav);
            //}
        }



        #endregion
    }
}