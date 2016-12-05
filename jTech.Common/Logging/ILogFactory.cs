using System;

namespace jTech.Common.Logging
{
    public interface ILogFactory
    {
        ILogger GetLogger(object caller);
        ILogger GetLogger(Type type);
    }
}
