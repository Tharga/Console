using AwesomeAssertions;
using Tharga.Console.Commands.StartupCommands;
using Xunit;

namespace Tharga.Console.Tests
{
    // Windows splits the command line in the Run key at spaces, so a path with a space must be quoted.
    public class Startup_command_line_tests
    {
        [Fact]
        public void A_program_path_with_a_space_is_quoted()
        {
            var commandLine = StartupCommand.GetCommandLine(@"C:\Program Files\Sample\Sample.exe");

            commandLine.Should().Be("\"C:\\Program Files\\Sample\\Sample.exe\"");
        }

        [Fact]
        public void A_dll_path_with_a_space_is_quoted_after_dotnet()
        {
            var commandLine = StartupCommand.GetCommandLine(@"C:\Users\First Last\Sample\Sample.dll");

            commandLine.Should().Be("dotnet \"C:\\Users\\First Last\\Sample\\Sample.dll\"");
        }
    }
}
