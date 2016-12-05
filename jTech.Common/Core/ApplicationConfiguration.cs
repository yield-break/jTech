using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;

namespace jTech.Common.Core
{
    [Export (typeof(IConfiguration))]
    public class ApplicationConfiguration : IConfiguration
    {
        readonly Dictionary<string, string> _appSettingsCache = new Dictionary<string, string>();
        
        public string this[string index]
        {
            get
            {
                string value;
                if (! _appSettingsCache.TryGetValue(index, out value))
                {
                    _appSettingsCache[index] = value = ConfigurationManager.AppSettings[index] ?? String.Empty;
                }

                return value;
            }
        }

    }
}
