using CrimesWebApi.ServiceModel;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrimesWebApi
{
    public class HomeService : Service
    {
        public object Any(HelloRequest request)
        {
            return new HelloResponse { Result = $"Hello {request.Name}"};
        }

    }
}
