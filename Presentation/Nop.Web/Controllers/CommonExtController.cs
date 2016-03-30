using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public partial class CommonController
    {
        // GET: CommonExt
        //[ChildActionOnly]
        public ActionResult Navigation()
        {
            return PartialView();
        }
    }
}