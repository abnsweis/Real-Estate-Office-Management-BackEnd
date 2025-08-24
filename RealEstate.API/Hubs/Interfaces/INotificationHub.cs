namespace RealEstate.API.Hubs.Interfaces
{
    public interface INotificationHub
    {
        Task Received(string message);
    }
}
