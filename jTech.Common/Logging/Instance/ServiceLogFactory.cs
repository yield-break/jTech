using System;
using System.ComponentModel.Composition;
using jTech.Common.Reactive;
using jTech.Common.Reactive.Events;

namespace jTech.Common.Logging.Instance
{
    [Export(typeof(ILogFactory))]
    public class ServiceLogFactory : LogFactory
    {
        private readonly IEventPublisher<IServiceRequest> _serviceEventPublisher;

        [ImportingConstructor]
        public ServiceLogFactory(IEventPublisher<IServiceRequest> serviceEventPublisher)
        {
            _serviceEventPublisher = serviceEventPublisher;
        }

        public override ILogger GetLogger(Type caller)
        {
            return new FileLogger(caller, _serviceEventPublisher, new ConsoleLogger(caller));
        }

    }
}
