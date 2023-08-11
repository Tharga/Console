using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using System.Threading.Tasks;
using Tharga.Console.Consoles;
using Tharga.Console.Interfaces;
using System.Speech.Synthesis;

namespace Tharga.Console.Commands;

public class VoiceConsole : ClientConsole
{
    private readonly SpeechRecognitionEngine _mainSpeechRecognitionEngine = new();
    private IRootCommand _rootCommand;
    private readonly VoiceConsoleConfiguration _consoleConfiguration;

    public VoiceConsole(VoiceConsoleConfiguration consoleConfiguration = null)
        : base(consoleConfiguration)
    {
        if (_mainSpeechRecognitionEngine == null) throw new InvalidOperationException("Cannot use SpeechRecognitionEngine.");
        _consoleConfiguration = consoleConfiguration ?? new VoiceConsoleConfiguration();

        //_mainSpeechRecognitionEngine.AudioLevelUpdated += _mainSpeechRecognitionEngine_AudioLevelUpdated;
        //_mainSpeechRecognitionEngine.AudioSignalProblemOccurred += _mainSpeechRecognitionEngine_AudioSignalProblemOccurred;
        //_mainSpeechRecognitionEngine.AudioStateChanged += _mainSpeechRecognitionEngine_AudioStateChanged;
        _mainSpeechRecognitionEngine.EmulateRecognizeCompleted += _mainSpeechRecognitionEngine_EmulateRecognizeCompleted;
        _mainSpeechRecognitionEngine.LoadGrammarCompleted += _mainSpeechRecognitionEngine_LoadGrammarCompleted;
        _mainSpeechRecognitionEngine.RecognizeCompleted += _mainSpeechRecognitionEngine_RecognizeCompleted;
        _mainSpeechRecognitionEngine.RecognizerUpdateReached += _mainSpeechRecognitionEngine_RecognizerUpdateReached;
        //_mainSpeechRecognitionEngine.SpeechDetected += _mainSpeechRecognitionEngine_SpeechDetected;
        //_mainSpeechRecognitionEngine.SpeechHypothesized += _mainSpeechRecognitionEngine_SpeechHypothesized;
        _mainSpeechRecognitionEngine.SpeechRecognitionRejected += _mainSpeechRecognitionEngine_SpeechRecognitionRejected;
        _mainSpeechRecognitionEngine.SpeechRecognized += _mainSpeechRecognitionEngine_SpeechRecognized;
    }

    private void _mainSpeechRecognitionEngine_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
    {
        OutputDefault("_mainSpeechRecognitionEngine_AudioLevelUpdated");
    }

    private void _mainSpeechRecognitionEngine_AudioSignalProblemOccurred(object sender, AudioSignalProblemOccurredEventArgs e)
    {
        OutputDefault("_mainSpeechRecognitionEngine_AudioSignalProblemOccurred");
    }

    private void _mainSpeechRecognitionEngine_AudioStateChanged(object sender, AudioStateChangedEventArgs e)
    {
        OutputDefault("_mainSpeechRecognitionEngine_AudioStateChanged");
    }

    private void _mainSpeechRecognitionEngine_EmulateRecognizeCompleted(object sender, EmulateRecognizeCompletedEventArgs e)
    {
        OutputDefault("_mainSpeechRecognitionEngine_EmulateRecognizeCompleted");
    }

    private void _mainSpeechRecognitionEngine_LoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
    {
        OutputDefault("_mainSpeechRecognitionEngine_LoadGrammarCompleted");
    }

    private void _mainSpeechRecognitionEngine_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
    {
        OutputDefault("_mainSpeechRecognitionEngine_RecognizeCompleted");
    }

    private void _mainSpeechRecognitionEngine_RecognizerUpdateReached(object sender, RecognizerUpdateReachedEventArgs e)
    {
        OutputDefault("_mainSpeechRecognitionEngine_RecognizerUpdateReached");
    }

    private void _mainSpeechRecognitionEngine_SpeechDetected(object sender, SpeechDetectedEventArgs e)
    {
        OutputDefault("_mainSpeechRecognitionEngine_SpeechDetected");
    }

    private void _mainSpeechRecognitionEngine_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
    {
        OutputDefault("_mainSpeechRecognitionEngine_SpeechHypothesized");
    }

    private void _mainSpeechRecognitionEngine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
    {
        if (!ApplicationIsActivated() && _consoleConfiguration.OnlyActiveWhenInFocus) return;

        var alternates = e.Result?.Alternates.Select(x => x.Text) ?? Array.Empty<string>();
        OutputDefault($"_mainSpeechRecognitionEngine_SpeechRecognitionRejected: {string.Join(",", alternates)}");
    }

    private void _mainSpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
    {
        if (!ApplicationIsActivated() && _consoleConfiguration.OnlyActiveWhenInFocus) return;

        //TODO: Execute the statement as a command (or input)
        OutputInformation(e.Result.Text);
        ConsoleManager.KeyInputEngine.Feed(e.Result.Text);
    }

    public override void Attach(CommandEngine commandEngine)
    {
        //TODO: When using commanding 'some option' it should be possible to.
        //- Tab
        //- Cancel or Escape
        //- Also start listening for words that are options so the can be selected by voice.
        //TODO: When using commanding 'some item' it should be possible to.
        //- Cancel or Escape
        //- Also start listening for any word freely as input.
        var coreCommands = new[] { "help" };
        var baseCommands = GetCommands(_rootCommand.SubCommands);

        var commands = coreCommands.Union(baseCommands).ToArray();
        if (Debugger.IsAttached)
        {
            foreach (var command in commands)
            {
                OutputInformation(command);
            }
        }

        var choices = new Choices();
        choices.Add(commands);
        var gr = new Grammar(new GrammarBuilder(choices));
        _mainSpeechRecognitionEngine.RequestRecognizerUpdate();
        _mainSpeechRecognitionEngine.LoadGrammar(gr);

        //TODO: Have sound feedback when using the app. (This is just a hello world test)
        if (_consoleConfiguration.UseOutputSound)
        {
            var ss = new SpeechSynthesizer();
            var c = ss.GetInstalledVoices();
            OutputInformation("Voices:");
            foreach (var installedVoice in c)
            {
                OutputInformation($"- {installedVoice.VoiceInfo.Name}");
            }

            ss.SelectVoice(c.Last().VoiceInfo.Name);
            ss.Speak($"Hello, I am {c.Last().VoiceInfo.Name.Replace("Microsoft ", "").Replace(" Desktop", "")}.");

            //    _mainSpeechRecognitionEngine.SetInputToWaveFile("helloworld.wav");
            //    synthesizer.Speak(builder);

            //    using (var synthesizer = new SpeechSynthesizer())
            //        builder.EndSentence();
            //    builder.AppendText(value);
            //    builder.StartSentence();

            //    var builder = new PromptBuilder();
            //    base.WriteLineEx(value, outputLevel);
        }

        try
        {
            _mainSpeechRecognitionEngine.SetInputToDefaultAudioDevice();
        }
        catch (Exception exception)
        {
            OutputError(exception);
        }

        foreach (var x in SpeechRecognitionEngine.InstalledRecognizers())
        {
            OutputInformation(x.Name);
        }

        Task.Run(() =>
        {
            //TODO: Terminate when application exists
            //TODO: This is the main recognizer, only use it when waiting for a main command. Not a command parameter input.
            while (true)
            {
                if (!ApplicationIsActivated() && _consoleConfiguration.OnlyActiveWhenInFocus)
                {
                    OutputInformation("Recognize Main Input...");
                }

                _mainSpeechRecognitionEngine.Recognize();
            }
        });
    }

    public override void Attach(IRootCommand rootCommand)
    {
        _rootCommand = rootCommand;
    }

    private static IEnumerable<string> GetCommands(IEnumerable<ICommand> commands)
    {
        if (commands == null) yield break;
        foreach (var cmd in commands)
        {
            var cmt = cmd as IContainerCommand;
            var subs = GetCommands(cmt?.SubCommands).ToArray();

            foreach (var name in cmd.Names)
            {
                yield return name;
                yield return name + " help";

                foreach (var sub in subs)
                {
                    yield return name + " " + sub;
                }
            }
        }
    }

    private static bool ApplicationIsActivated()
    {
        var activatedHandle = GetForegroundWindow();
        if (activatedHandle == IntPtr.Zero)
        {
            return false;
        }

        var procId = Process.GetCurrentProcess();
        GetWindowThreadProcessId(activatedHandle, out var activeProcId);
        return activeProcId == procId.Id;
    }

    #region User32

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

    #endregion
}