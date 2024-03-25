namespace CompanyHub.Services.Interfaces
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        bool SetData <T>(string key, T value,DateTimeOffset expirationTime);
        object RemoveData<T>(string key);
    }
}
