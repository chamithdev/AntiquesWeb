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
            var vendors = _vendorService.GetAllVendors();
            var customers = _customerService.GetAllCustomers(createdFromUtc: DateTime.Now.AddMonths(-2), createdToUtc: DateTime.Now);
            var shops = new List<VendorModel>();
            foreach (var c in customers)
            {
                if(c.VendorId>0)
                {

                    var vendor = _vendorService.GetVendorById(c.VendorId);
                    var vendorModel = new VendorModel
                    {
                        Id = vendor.Id,
                        Name = vendor.GetLocalized(x => x.Name),
                        Description = vendor.GetLocalized(x => x.Description),
                        MetaKeywords = vendor.GetLocalized(x => x.MetaKeywords),
                        MetaDescription = vendor.GetLocalized(x => x.MetaDescription),
                        MetaTitle = vendor.GetLocalized(x => x.MetaTitle),
                        SeName = vendor.GetSeName(),                       
                        ImageUrl = _pictureService.GetPictureUrl(vendor.PictureId, 200)
                    };
                    shops.Add(vendorModel);
                }
                
            }

            return View("_LatestBoutique", shops);
        }
    }
}