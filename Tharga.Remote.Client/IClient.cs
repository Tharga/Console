using System.Threading.Tasks;

namespace Tharga.Remote.Client
{
    public interface IClient
    {
        public Task ConnectAsync();
    }
}