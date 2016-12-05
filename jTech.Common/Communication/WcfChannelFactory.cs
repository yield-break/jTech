using System;
using System.ServiceModel;

namespace jTech.Common.Communication
{
    public static class WcfChannelFactory<T>
    {
        public static void Execute(string endpointConfigurationName, Action<T> action)
        {
            Execute(endpointConfigurationName,
            channel =>
            {
                action(channel);
                return true;
            });
        }

        public static TResult Execute<TResult>(string endpointConfigurationName, Func<T, TResult> func)
        {
            using (var factory = new ChannelFactory<T>(endpointConfigurationName))
            {
                T channel = factory.CreateChannel();
                return func(channel);
            }
        }

    }
}
