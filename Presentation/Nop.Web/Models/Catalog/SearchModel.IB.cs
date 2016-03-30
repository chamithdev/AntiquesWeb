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
        
    }
}