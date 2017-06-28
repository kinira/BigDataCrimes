using Funq;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Redis;
using System.IO;

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
        }
    }
}