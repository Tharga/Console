using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Command.Base
{
    public class VoiceConsole : SystemConsoleBase
    {
        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        private static extern bool PostMessage(IntPtr hwnd, uint msg, int wparam, int lparam);

        private const int Retrun = 0x0D;
        private const int Keydown = 0x100;

        private enum InputMethod
        {
            Unknown,
            Keyboard,
            Voice
        }

        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private readonly SpeechRecognitionEngine _mainSpeechRecognitionEngine = new SpeechRecognitionEngine();
        private readonly SpeechRecognitionEngine _subSpeechRecognitionEngine = new SpeechRecognitionEngine();
        private string _input;
        private InputMethod _inputMethod;
        private bool _reading;
        private ConsoleKeyInfo _keyInput;

        public override void Initiate(IEnumerable<string> commandKeys)
        {
            var choices = new Choices();
            choices.Add(commandKeys.ToArray());
            var gr = new Grammar(new GrammarBuilder(choices));
            _mainSpeechRecognitionEngine.RequestRecognizerUpdate();
            _mainSpeechRecognitionEngine.LoadGrammar(gr);
            _mainSpeechRecognitionEngine.SpeechRecognized += _mainSpeechRecognitionEngine_SpeechRecognized;
            _mainSpeechRecognitionEngine.SetInputToDefaultAudioDevice();

            var subChoices = new Choices();
            subChoices.Add(new[] { "tab", "enter" });
            var subGr = new Grammar(new GrammarBuilder(subChoices));
            _subSpeechRecognitionEngine.RequestRecognizerUpdate();
            _subSpeechRecognitionEngine.LoadGrammar(subGr);
            _subSpeechRecognitionEngine.SpeechRecognized += _subSpeechRecognitionEngine_SpeechRecognized;
            _subSpeechRecognitionEngine.SetInputToDefaultAudioDevice();
        }

        protected override void WriteLine(string value)
        {
            System.Console.WriteLine(value);

            //var builder = new PromptBuilder();
            //builder.StartSentence();
            //builder.AppendText(value);
            //builder.EndSentence();

            //using (var synthesizer = new SpeechSynthesizer())
            //{
            //    //synthesizer.SelectVoice("Microsoft David Desktop");
            //    //synthesizer.SelectVoice("Microsoft Hazel Desktop");
            //    synthesizer.SelectVoice("Microsoft Zira Desktop");
            //    synthesizer.Speak(builder);
            //}
        }

        private void _mainSpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (_reading)
            {
                _inputMethod = InputMethod.Voice;
                _input = e.Result.Text;
                System.Console.Write(_input);
                _autoResetEvent.Set();
            }
        }

        private void _subSpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (_reading)
            {
                _inputMethod = InputMethod.Voice;
                _input = e.Result.Text;

                var hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                switch (_input)
                {
                    case "tab":
                        PostMessage(hwnd, Keydown, 9, 0);
                        break;
                    case "enter":
                        PostMessage(hwnd, Keydown, 13, 0);
                        break;
                }
            }
        }

        public override string ReadLine()
        {
            _inputMethod = InputMethod.Unknown;

            var task = Task.Factory.StartNew(() =>
            {
                var s = System.Console.ReadLine();
                _inputMethod = InputMethod.Keyboard;
                if (!_reading) return;
                _input = s;
                _autoResetEvent.Set();
            });

            _mainSpeechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);

            _reading = true;
            _autoResetEvent.WaitOne();
            _reading = false;

            _mainSpeechRecognitionEngine.RecognizeAsyncCancel();

            if (_inputMethod == InputMethod.Voice)
            {
                var hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                PostMessage(hwnd, Keydown, Retrun, 0);
            }

            task.Wait();
            task.Dispose();

            return _input;
        }

        public override ConsoleKeyInfo ReadKey()
        {
            _inputMethod = InputMethod.Unknown;

            var task = Task.Factory.StartNew(() =>
            {
                var s = System.Console.ReadKey();
                _inputMethod = InputMethod.Keyboard;
                if (!_reading) return;
                _keyInput = s;
                _autoResetEvent.Set();
            });

            _subSpeechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);

            _reading = true;
            _autoResetEvent.WaitOne();
            _reading = false;

            _subSpeechRecognitionEngine.RecognizeAsyncCancel();

            ////if (_inputMethod == InputMethod.Voice)
            ////{
            ////    if (_input == "tab")
            ////        _keyInput = new ConsoleKeyInfo((char)10, ConsoleKey.Tab, false, false, false);

            ////    var hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            ////    PostMessage(hWnd, WM_KEYDOWN, _keyInput.KeyChar, 0);
            ////}

            task.Wait();
            task.Dispose();

            //return _keyInput;
            _reading = false;
            return _keyInput;
        }
    }
}