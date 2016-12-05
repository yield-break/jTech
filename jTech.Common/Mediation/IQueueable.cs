
namespace jTech.Common.Mediation
{
    public interface IQueueable<out T>
    {
        long QueueId { get; }
        T Message { get; }
    }
}
