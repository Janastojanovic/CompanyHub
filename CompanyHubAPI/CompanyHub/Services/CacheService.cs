using CompanyHub.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace CompanyHub.Services
{
    public class CacheService : ICacheService
    {
        private IDatabase _cacheDb;
        //private IConnectionMultiplexer redis;
        //public CacheService(IDatabase cacheDb, IConnectionMultiplexer redis) 
        //{
        //    //var redis = ConnectionMultiplexer.Connect("redis-19108.c267.us-east-1-4.ec2.cloud.redislabs.com:19108");
        //    _cacheDb = redis.GetDatabase(); 
        //}
        public CacheService(IConnectionMultiplexer redis)
        {
            _cacheDb = redis.GetDatabase();
        }
        public T GetData<T>(string key)
        {
           var value = _cacheDb.StringGet(key);
            if(!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public object RemoveData<T>(string key)
        {
            var exists= _cacheDb.KeyExists(key);
            if(exists)
            {
                return _cacheDb.KeyDelete(key);
            }
            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expirtyTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return _cacheDb.StringSet(key,JsonSerializer.Serialize(value),expirtyTime);
        }
    }
}
