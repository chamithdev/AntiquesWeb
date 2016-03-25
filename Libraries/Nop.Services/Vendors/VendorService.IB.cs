using Nop.Core;
using Nop.Core.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Vendors
{
    public partial class VendorService
    {

        public virtual IPagedList<Vendor> GetAllVendorsByDateRange(DateTime datefromUtc,DateTime dateToUtc,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query =(from v in _vendorRepository.TableNoTracking
                 join c in _customerRepository.TableNoTracking on v.Id equals c.VendorId
                        where v.Active && !v.Deleted && c.CreatedOnUtc >= datefromUtc && c.CreatedOnUtc <= dateToUtc
                 select v
                 );
          

            query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name);

            var vendors = new PagedList<Vendor>(query, pageIndex, pageSize);
            return vendors;
        }


        public virtual IPagedList<Vendor> GetAllVendorsOrderByDate(string name = "",
            bool isDecendin=true, int pageIndex = 0, int pageSize = int.MaxValue, 
            bool showHidden = false)
        {
            var query = (from v in _vendorRepository.TableNoTracking
                         join c in _customerRepository.TableNoTracking on v.Id equals c.VendorId
                         where v.Active && !v.Deleted
                         orderby c.CreatedOnUtc descending
                         select v
                 );
                       
            if(!isDecendin)
            {
                query = (from v in _vendorRepository.TableNoTracking
                         join c in _customerRepository.TableNoTracking on v.Id equals c.VendorId
                         where v.Active && !v.Deleted
                         orderby c.CreatedOnUtc
                         select v
                 );
            }
            if(!string.IsNullOrWhiteSpace(name))
                query = query.Where(v => v.Name.Contains(name));

            var vendors = new PagedList<Vendor>(query, pageIndex, pageSize);
            return vendors;
        }
    }
}
