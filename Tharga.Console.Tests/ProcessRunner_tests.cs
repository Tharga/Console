using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AwesomeAssertions;
using Tharga.Console.Helpers;
using Xunit;

namespace Tharga.Console.Tests;

public class ProcessRunner_tests
{
    private const string Line = "0123456789012345678901234567890123456789012345678901234567890123";
    private const int LineCount = 3000; // About 200 KB, well over the pipe buffer on Windows and Linux.

    [Fact]
    public async Task Output_larger_than_the_pipe_buffer_is_returned_without_hanging()
    {
        var (fileName, arguments) = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? ("cmd.exe", $"/C for /L %i in (1,1,{LineCount}) do @echo {Line}")
            : ("/bin/sh", $"-c \"i=0; while [ $i -lt {LineCount} ]; do echo {Line}; i=$((i+1)); done\"");

        var output = await Task.Run(() => ProcessRunner.Run(fileName, arguments))
            .WaitAsync(TimeSpan.FromSeconds(30), TestContext.Current.CancellationToken);

        var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        lines.Should().HaveCount(LineCount);
        lines.Should().OnlyContain(x => x == Line);
    }
}
