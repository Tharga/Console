using System;
using System.Collections.Generic;

namespace Tharga.Console.Consoles;

public sealed class ConsoleReadContext
{
    public ConsoleReadContext(string prompt, Func<string, CompletionResult> complete)
    {
        Prompt = prompt ?? string.Empty;
        Complete = complete ?? throw new ArgumentNullException(nameof(complete));
    }

    public string Prompt { get; }
    public Func<string, CompletionResult> Complete { get; }
}

public sealed class CompletionResult
{
    public CompletionResult(string baseText, IReadOnlyList<string> candidates)
    {
        BaseText = baseText ?? string.Empty;
        Candidates = candidates ?? Array.Empty<string>();
    }

    public string BaseText { get; }
    public IReadOnlyList<string> Candidates { get; }
}
