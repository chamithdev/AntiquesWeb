using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;

namespace Nop.Core.Domain.Vendors
{
    public partial class Vendor
    {

        public bool ShowOnHomePage { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Web { get; set; }

        public int PictureId { get; set; }

        public int PictureId2 { get; set; }
        
    }
}
