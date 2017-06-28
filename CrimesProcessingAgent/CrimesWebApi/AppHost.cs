using Crimes.Processing;
using Crimes.Processing.Predictions;
using Funq;
using Grpc.Core;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Redis;
using System.Collections.Generic;
using System.IO;
using static CrimesProcessing.Contracts.CrimesService;

namespace CrimesWebApi
{
    public class AppHost : AppHostBase
    {
        /// <summary>
        /// Configure your ServiceStack AppHost singleton instance:
        /// Call base constructor with App Name and assembly where Service classes are located
        /// </summary>
        public AppHost()
            : base("RedisGeo", typeof(HomeService).GetAssembly())
        {
            AppSettings = new MultiAppSettings(
                new EnvironmentVariableSettings(),
                new AppSettings());
        }

        public override void Configure(Container container)
        {
            container.Register<IRedisClientsManager>(c =>
                new RedisManagerPool(AppSettings.Get("REDIS_HOST", defaultValue: "localhost")));

            container.Register(c => PrepareAgents());

            container.AddSingleton<PositionCalculator>();
            container.AddScoped<IStatisticProvider>(c=> new StatisticProvider());
        }

        private IReadOnlyList<CrimesServiceClient> PrepareAgents()
        {
            var channels = new[]
            {
                 new Channel($"127.0.0.1:50051", ChannelCredentials.Insecure),
                 new Channel($"127.0.0.1:50051", ChannelCredentials.Insecure),
                 new Channel($"127.0.0.1:50051", ChannelCredentials.Insecure)
            };

            var clients = new[]
            {
                new CrimesServiceClient(channels[0]),
                new CrimesServiceClient(channels[1]),
                new CrimesServiceClient(channels[2])
            };

            return clients;
        }
    }
}