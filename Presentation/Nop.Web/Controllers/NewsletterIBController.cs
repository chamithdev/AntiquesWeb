using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Models.Newsletter;

namespace Nop.Web.Controllers
{
    public partial class NewsletterController
    {
        [ChildActionOnly]
        public ActionResult NewsletterPopup()
        {
            if (_customerSettings.HideNewsletterBlock)
                return Content("");

            var model = new NewsletterBoxModel()
            {
                AllowToUnsubscribe = _customerSettings.NewsletterBlockAllowToUnsubscribe
            };
            return PartialView(model);
        }

    }
}