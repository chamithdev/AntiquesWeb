using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Catalog
{
    public class ProductInquieryModel
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Message { get; set; }

        public int VendorId { get; set; }

        public bool Subscribe { get; set; }
    }
}