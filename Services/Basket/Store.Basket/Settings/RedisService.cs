using StackExchange.Redis;
using System;

namespace Store.Basket.Settings
{
    public class RedisService
    {
        public string _host { get; set; }
        public int _port { get; set; }

        private ConnectionMultiplexer? _connectionMultiplexer;
        private readonly object _lock = new();

        public RedisService(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public bool IsConnected => _connectionMultiplexer != null && _connectionMultiplexer.IsConnected;

        public void Connect()
        {
            try
            {
                lock (_lock)
                {
                    if (_connectionMultiplexer == null || !_connectionMultiplexer.IsConnected)
                    {
                        var config = new ConfigurationOptions
                        {
                            EndPoints = { $"{_host}:{_port}" },
                            AbortOnConnectFail = false,
                            ConnectTimeout = 1500,
                            SyncTimeout = 1500,
                            AsyncTimeout = 1500,
                            KeepAlive = 60
                        };
                        _connectionMultiplexer = ConnectionMultiplexer.Connect(config);
                    }
                }
            }
            catch
            {
                // Redis bağlantı hatası durumunda sessizce devam edilir
            }
        }

        public IDatabase? GetDb(int db = 0)
        {
            if (_connectionMultiplexer == null || !_connectionMultiplexer.IsConnected)
            {
                Connect();
            }

            if (_connectionMultiplexer != null && _connectionMultiplexer.IsConnected)
            {
                return _connectionMultiplexer.GetDatabase(db);
            }

            return null;
        }
    }
}
