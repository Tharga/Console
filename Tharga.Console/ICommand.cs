using System.Threading.Tasks;

namespace Tharga.Console;

public interface ICommand
{
    string Name { get; }
    string Description { get; }
    Task ExecuteAsync();
}
