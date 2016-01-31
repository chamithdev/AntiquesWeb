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



       
    }
}