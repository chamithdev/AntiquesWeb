using Nop.Core;
using Nop.Core.Domain.Vendors;
using System;


namespace Nop.Services.Vendors
{
    public partial interface IVendorService
    {

        IPagedList<Vendor> GetAllVendorsByDateRange(DateTime datefromUtc,DateTime dateToUtc,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        IPagedList<Vendor> GetAllVendorsOrderByDate(string name = "",
           bool isDecendin = true, int pageIndex = 0, int pageSize = int.MaxValue,
           bool showHidden = false);
    }
}
