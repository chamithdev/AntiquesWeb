using System.Web.Mvc;
using Nop.Web.Framework.Security;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LatestFinds()
        {
            return View();
        }
        
        public ActionResult ComingSoon()
        {
            return View();
        }

        public ActionResult TermsAndConditions()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }

        
    }
}
