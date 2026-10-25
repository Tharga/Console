using Xunit;

namespace Tharga.Console.Tests
{
    // Tests that change state shared by the whole process, such as the System.Console streams,
    // run in this collection: one at a time, and not alongside the parallel tests.
    [CollectionDefinition(Name, DisableParallelization = true)]
    public class ProcessStateCollection
    {
        public const string Name = "Process state";
    }
}
