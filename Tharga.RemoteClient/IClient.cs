using System.Threading.Tasks;

namespace Tharga.RemoteClient
{
    public interface IClient
    {
        public Task ConnectAsync();
    }
}