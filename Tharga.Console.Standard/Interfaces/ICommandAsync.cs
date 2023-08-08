using System.Threading.Tasks;

namespace Tharga.Console.Interfaces
{
    public interface ICommandAsync
    {
        Task InvokeAsync(string[] param);
    }
}