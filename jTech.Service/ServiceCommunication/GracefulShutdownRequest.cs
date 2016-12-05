using jTech.Common.Reactive.Events;

namespace jTech.Service.ServiceCommunication
{
    public class GracefulShutdownRequest : ServiceRequest
    {
        public GracefulShutdownRequest(object sender, string reason) : base(sender, reason) { }

    }
}
