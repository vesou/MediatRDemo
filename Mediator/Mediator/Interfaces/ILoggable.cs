namespace Mediator.Interfaces
{
    public interface ILoggable
    {
        (string Message, object Data) ToLogMessage();
    }
}