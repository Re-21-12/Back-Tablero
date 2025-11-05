public interface ISocketService
{
    Task<bool> SendEventAsync(string typeEvent, object data);
}