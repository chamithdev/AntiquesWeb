using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Models.Catalog
{
    public partial class SearchModel
    {
        private IList<SelectListItem> _vendors;

        public IList<SelectListItem> Vendors
        {
            get { return _vendors ?? new List<SelectListItem>(); }
            set { _vendors = value; }
        }

        private IList<SelectListItem> _subCats;

        public IList<SelectListItem> SubCategories
        {
            get { return _subCats ?? new List<SelectListItem>(); ; }
            set { _subCats = value; }
        }

        private IList<SelectListItem> _customData;

        public IList<SelectListItem> CustomData
        {
            get { return _customData ?? new List<SelectListItem>(); ; }
            set { _customData = value; }
        }

        public decimal SizeFrom { get; set; }
        public decimal SizeTo { get; set; }

        private IList<SelectListItem> _dimension;

        public IList<SelectListItem> Dimension
        {
            get { return _dimension ?? new List<SelectListItem>(); ; }
            set { _dimension = value; }
        }
        public int SelectedDimension { get; set; }

        public List<Category> ProductCategories { get; set; }

        public decimal pm { get; set; }
        public decimal pmx { get; set; }
        public decimal hm { get; set; }
        public decimal hmx { get; set; }

        public decimal wm { get; set; }
        public decimal wmx { get; set; }

        public decimal PriceMinAvailable { get; set; }
        public decimal PriceMaxAvailable { get; set; }
        public decimal HeightMinAvailable { get; set; }
        public decimal HeightMaxAvailable { get; set; }

        public decimal WidthMinAvailable { get; set; }
        public decimal WidthMaxAvailable { get; set; }

        public List<CustomData> Styles { get; set; }

        public List<CustomData> Materials { get; set; }

        public string c { get; set; }
        public string d { get; set; }

        public IList<SelectListItem> Colors { get; set; }
        public IList<SelectListItem> Designers { get; set; }

        

        public List<string> ss { get; set; }

        public List<string> sms { get; set; }

        public string cdf { get; set; }

        public string cdt { get; set; }

        public int pg { get; set; }

    }
}