using ServiceStack;

namespace CrimesWebApi.ServiceModel
{
    [Route("/hello/{Name}")]
    public class HelloRequest : IReturn<HelloResponse>
    {
        public string Name { get; set; }

    }

}
