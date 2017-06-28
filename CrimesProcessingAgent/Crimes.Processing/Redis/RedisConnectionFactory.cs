using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimes.Processing.Redis
{
    public class RedisConnectionFactory
    {

        private static readonly Lazy<ConnectionMultiplexer> Connection;

        static RedisConnectionFactory()
        {
            var connectionString = "redis-17259.c10.us-east-1-4.ec2.cloud.redislabs.com:17259";
            var options = ConfigurationOptions.Parse(connectionString);
            Connection = new Lazy<ConnectionMultiplexer>(() =>
               ConnectionMultiplexer.Connect(options)
            );
        }

        public static ConnectionMultiplexer GetConnection() => Connection.Value;

    }

}
