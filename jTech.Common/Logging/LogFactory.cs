using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jTech.Common.Logging
{
    public abstract class LogFactory : ILogFactory
    {
        public ILogger GetLogger(object caller)
        {
            return GetLogger(caller.GetType());
        }
        public abstract ILogger GetLogger(Type caller);

    }
}
