using System.Speech.Recognition;
using Microsoft.Extensions.Options;

namespace Tharga.Console.Consoles;

/// <summary>
/// This console can output text and execute commands using voice input and output.
/// </summary>
public class VoiceConsole : ClientConsole
{
    private readonly SpeechRecognitionEngine _mainSpeechRecognitionEngine = new();
    private readonly VoiceConsoleConfiguration _voiceConsoleConfiguration;

    public VoiceConsole(IOptions<VoiceConsoleConfiguration> voiceConsoleConfiguration = default)
    {
        _voiceConsoleConfiguration = voiceConsoleConfiguration?.Value;
    }
}