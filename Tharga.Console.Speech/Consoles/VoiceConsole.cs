using System;
using System.Speech.Recognition;
using Tharga.Console.Consoles;

namespace Tharga.Console.Consoles;

public sealed class VoiceConsole : IConsoleControl
{
    private readonly SpeechRecognitionEngine _engine;

    public VoiceConsole()
    {
        _engine = new SpeechRecognitionEngine();
        _engine.SetInputToDefaultAudioDevice();
        _engine.LoadGrammar(new DictationGrammar());
    }

    public bool CanRead => true;

    public bool SupportsColors => true;

    public string ReadLine(ConsoleReadContext context, out bool eof)
    {
        eof = false;
        var result = _engine.Recognize();
        return result?.Text ?? string.Empty;
    }

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
