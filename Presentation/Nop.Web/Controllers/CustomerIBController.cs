using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Services.Localization;
using Nop.Services.Seo;
namespace Nop.Web.Controllers
{
    public partial class CustomerController
    {
        

        [ChildActionOnly]
        public ActionResult LatestBoutique()
        {
            //var vendors = _vendorService.GetAllVendors();
            var vendors = _vendorService.GetAllVendorsByDateRange(datefromUtc: DateTime.Now.AddMonths(-2), dateToUtc: DateTime.Now);

            //var customers = _customerService.GetAllCustomers(createdFromUtc: DateTime.Now.AddMonths(-2), createdToUtc: DateTime.Now).Where(c => c.VendorId > 0).Take(12);
            //customers = customers.OrderBy(c => Guid.NewGuid());
            var selectedvendors = vendors.OrderBy(v => Guid.NewGuid()).ToList().Take(12);

            var shops = new List<VendorModel>();
            foreach (var vendor in selectedvendors)
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
                    ImageUrl = _pictureService.GetPictureUrl(vendor.PictureId, 200),
                    ImageUrl2 = vendor.PictureId2 > 0 ? _pictureService.GetPictureUrl(vendor.PictureId2, 200) : _pictureService.GetPictureUrl(vendor.PictureId, 200)
                };

                shops.Add(vendorModel);
                
            }

            return View("_LatestBoutique", shops);
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        

    }
}