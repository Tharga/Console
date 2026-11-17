using System;
using System.Linq;
using System.Threading;
using AwesomeAssertions;
using Tharga.Console.Helpers;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_feeding_text_to_the_key_input_engine
    {
        [Fact]
        public void Should_keep_every_digit_as_a_digit_key()
        {
            //Arrange
            using var gate = new ManualResetEventSlim();
            var engine = new KeyInputEngine(() => { gate.Wait(); throw new InvalidOperationException(); });

            //Act
            engine.Feed("0123456789");
            var keys = Enumerable.Range(0, 10).Select(_ => engine.ReadKey(CancellationToken.None)).ToArray();
            gate.Set();

            //Assert
            keys.Select(x => x.KeyChar).Should().Equal("0123456789".ToCharArray());
            keys.Select(x => x.Key).Should().Equal(ConsoleKey.D0, ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5, ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9);
        }

        [Fact]
        public void Should_map_letters_and_space_and_end_with_enter()
        {
            //Arrange
            using var gate = new ManualResetEventSlim();
            var engine = new KeyInputEngine(() => { gate.Wait(); throw new InvalidOperationException(); });

            //Act
            engine.Feed("aZ -");
            var keys = Enumerable.Range(0, 5).Select(_ => engine.ReadKey(CancellationToken.None)).ToArray();
            gate.Set();

            //Assert
            keys.Select(x => x.KeyChar).Should().Equal('a', 'Z', ' ', '-', (char)13);
            keys.Select(x => x.Key).Take(3).Should().Equal(ConsoleKey.A, ConsoleKey.Z, ConsoleKey.Spacebar);
            keys[3].Key.Should().NotBe(ConsoleKey.Backspace).And.NotBe(ConsoleKey.Tab);
            keys[4].Key.Should().Be(ConsoleKey.Enter);
        }
    }
}
