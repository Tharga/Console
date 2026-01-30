using System;
using System.Collections.Generic;
using System.Text;

namespace Tharga.Console.Consoles;

public sealed class DefaultConsole : IConsoleControl
{
    public bool CanRead => true;

    public bool SupportsColors => true;

    public void Write(string text, ConsoleColor? color = null)
    {
        WriteInternal(text, color, newLine: false);
    }

    public void WriteLine(string text, ConsoleColor? color = null)
    {
        WriteInternal(text, color, newLine: true);
    }

    public void Clear()
    {
        System.Console.Clear();
    }

    public string ReadLine(ConsoleReadContext context, out bool eof)
    {
        if (System.Console.IsInputRedirected)
        {
            var line = System.Console.ReadLine();
            eof = line is null;
            return line ?? string.Empty;
        }

        eof = false;
        if (context.Prompt.Length > 0)
            System.Console.Write(context.Prompt);

        var buffer = new StringBuilder();
        var lastCandidates = new List<string>();
        var lastBase = string.Empty;
        var candidateIndex = -1;
        var lastRenderLength = 0;

        while (true)
        {
            var key = System.Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                System.Console.WriteLine();
                return buffer.ToString();
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (buffer.Length > 0)
                {
                    buffer.Length -= 1;
                    ResetCompletionState();
                    RedrawLine(buffer.ToString());
                }
                continue;
            }

            if (key.Key == ConsoleKey.Escape)
            {
                buffer.Clear();
                ResetCompletionState();
                RedrawLine(buffer.ToString());
                continue;
            }

            if (key.Key == ConsoleKey.Tab)
            {
                var direction = key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? -1 : 1;
                var input = buffer.ToString();

                var canCycle = lastCandidates.Count > 0 &&
                               candidateIndex >= 0 &&
                               string.Equals(input, lastBase + lastCandidates[candidateIndex],
                                   StringComparison.OrdinalIgnoreCase);

                if (!canCycle)
                {
                    var completion = context.Complete(input);
                    if (completion.Candidates.Count == 0)
                        continue;

                    lastCandidates = new List<string>(completion.Candidates);
                    lastBase = completion.BaseText;
                    candidateIndex = direction > 0 ? 0 : completion.Candidates.Count - 1;
                }
                else
                {
                    if (direction > 0)
                        candidateIndex = (candidateIndex + 1) % lastCandidates.Count;
                    else
                        candidateIndex = (candidateIndex - 1 + lastCandidates.Count) % lastCandidates.Count;
                }

                var selection = lastCandidates[candidateIndex];
                buffer.Clear();
                buffer.Append(lastBase);
                buffer.Append(selection);
                RedrawLine(buffer.ToString());
                continue;
            }

            if (!char.IsControl(key.KeyChar))
            {
                buffer.Append(key.KeyChar);
                ResetCompletionState();
                RedrawLine(buffer.ToString());
            }
        }

        void ResetCompletionState()
        {
            lastCandidates.Clear();
            lastBase = string.Empty;
            candidateIndex = -1;
        }

        void RedrawLine(string text)
        {
            var full = context.Prompt + text;
            var clearLen = Math.Max(lastRenderLength, full.Length);

            System.Console.Write("\r");
            if (clearLen > 0)
                System.Console.Write(new string(' ', clearLen));
            System.Console.Write("\r");
            System.Console.Write(full);
            lastRenderLength = full.Length;
        }
    }

    private static void WriteInternal(string text, ConsoleColor? color, bool newLine)
    {
        var previous = System.Console.ForegroundColor;
        if (color.HasValue)
            System.Console.ForegroundColor = color.Value;

        if (newLine)
            System.Console.WriteLine(text);
        else
            System.Console.Write(text);

        System.Console.ForegroundColor = previous;
    }
}
