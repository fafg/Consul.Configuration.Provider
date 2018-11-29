using System;
using System.Text;
using System.Threading.Tasks;

namespace Consul.Configuration.Gateway
{
    public class ConsulGateway : IConsulGateway
    {
        private static ConsulClient _client;

        public ConsulGateway() { }

        public void Init(string uri, string dataCenter, string token)
        {
            _client = new ConsulClient(c => 
            {
                c.Address = new Uri(uri);
                c.Datacenter = dataCenter;
                c.Token = token;
                //c.ClientCertificate = <NextVersion>;
                //c.HttpAuth = <NextVersion>;
                //c.WaitTime = <NextVersion>;
            });
        }

        public void Destroy()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }

        public async Task<string> GetValueAsync(string key)
        {
            string value = string.Empty;
       
            if (string.IsNullOrWhiteSpace(key))
                return value;

            try
            {
                QueryResult<KVPair> getPair = await _client.KV.Get(key);
                value = Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length);
            }
            catch (Exception ex)
            {
                value = $"ERROR ON TRYING TO GET KEY: {key}";
                Console.WriteLine(ex.Message);
            }

            return value;
        }
    }
}