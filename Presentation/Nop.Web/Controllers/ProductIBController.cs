using System;
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
            //ACL and store mapping
            if (id==1)
                products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            else
                products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).Take(10).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (products.Count == 0)
                return Content("");

            var model = PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();
            model = model.OrderBy(p => Guid.NewGuid()).ToList();
            return PartialView(model);
        }

       

    }
}