using FluentAssertions;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Xunit;

namespace Tharga.Console.Tests;

public class InputInstance_recovery_tests
{
    [Fact]
    public void Non_negative_buffer_position_is_returned_unchanged()
    {
        var start = new Location(2, 3);
        var screen = new Location(5, 7);

        var (newStart, newPos) = InputInstance.RecoverNegativeBufferPosition(4, screen, start);

        newStart.Should().BeSameAs(start);
        newPos.Should().Be(4);
    }

    [Fact]
    public void Negative_buffer_position_reanchors_start_to_current_screen_and_resets_position_to_zero()
    {
        var start = new Location(0, 10);
        var screen = new Location(3, 5);

        var (newStart, newPos) = InputInstance.RecoverNegativeBufferPosition(-7, screen, start);

        newStart.Should().BeSameAs(screen);
        newPos.Should().Be(0);
    }
}
