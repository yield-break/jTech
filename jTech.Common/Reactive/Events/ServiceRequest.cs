
namespace jTech.Common.Reactive.Events
{
    public interface IServiceRequest
    {
        object Sender { get; }
        string Reason { get; }
    }

    public abstract class ServiceRequest : IServiceRequest
    {
        public ServiceRequest(object sender, string reason)
        {
            Sender = sender;
            Reason = reason;
        }

        public object Sender { get; private set; }
        public string Reason { get; private set; }

    }
}
