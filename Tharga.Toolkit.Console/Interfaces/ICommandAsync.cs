using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface ICommandAsync
    {
        Task InvokeAsync(string[] param);
    }
}