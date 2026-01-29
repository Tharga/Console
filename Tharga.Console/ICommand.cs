using System.Threading.Tasks;

namespace Tharga.Console;

public interface ICommand
{
    string Name { get; }
    Task ExecuteAsync();
}
