using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Common;

namespace Nop.Services.Common
{
    public interface ICustomDataService
    {
        IList<CustomData> GetCustomDataByKeyGroup(string keyGroup);
    }
}
