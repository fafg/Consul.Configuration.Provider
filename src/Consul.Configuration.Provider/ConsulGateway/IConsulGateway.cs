using System;
using System.Text;
using System.Threading.Tasks;

namespace Consul.Configuration.Gateway
{
    public interface IConsulGateway
    {
        void Init(string uri, string dataCenter, string token);

        void Destroy();
        
        Task<string> GetValueAsync(string key);
    }
}
