using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Catalog
{
    public partial class ProductDetailsModel
    {


        public int Rank { get; set; }


        public string CircaDate { get; set; }

        public string Material { get; set; }

        public string Color { get; set; }

        public string Country { get; set; }

        public string DesignBy { get; set; }

        public string Condition { get; set; }

        public bool TermsCondtion { get; set; }

        public decimal Height { get; set; }

        public decimal Width { get; set; }

        public decimal Length { get; set; }

        public decimal Weight { get; set; }

        public int StockQuantity { get; set; }
    }
}