using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Events;
namespace Nop.Services.Common
{
    public partial class CustomDataService : ICustomDataService
    {
        private const string GENERICATTRIBUTE_KEY = "Nop.customdata.{0}";
        #region Fields

        private readonly IRepository<CustomData> _customDataRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion
        #region Ctor
        public CustomDataService(ICacheManager cacheManager,
            IRepository<CustomData> customDataRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._customDataRepository = customDataRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion
         #region Methods
        public IList<CustomData> GetCustomDataByKeyGroup(string keyGroup)
        {
            string key = string.Format(GENERICATTRIBUTE_KEY, keyGroup);
            return _cacheManager.Get(key, () =>
            {
                var query = from ga in _customDataRepository.Table
                            where ga.KeyGroup == keyGroup
                            select ga;
                var customData = query.ToList();
                return customData;
            });
        }
#endregion
    }
}
