using ServiceStack;

namespace CrimesWebApi.ServiceModel
{
    public class HelloResponse
    {
        public string Result { get; set; }

        public ResponseStatus ResponseStatus { get; set; } //Automatic exception handling
    }
}
