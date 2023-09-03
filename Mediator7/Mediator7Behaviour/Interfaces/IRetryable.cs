namespace Mediator7Behaviour.Interfaces
{
    public interface IRetryable
    {
        // this could store more info about how and what to retry e.g. types of exceptions or fallback time
        public int MaxRetryCount { get; }
    }
}