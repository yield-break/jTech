using System;

namespace jTech.Common.Logging
{
    public interface ILogger
    {
        void Log(LogCatagory catagory, string message);
        void Log(LogCatagory catagory, string message, params object[] args);
        void Log(LogCatagory catagory, Exception e, string message);
        void Log(LogCatagory catagory, Exception e, string message, params object[] args);
    }
}
