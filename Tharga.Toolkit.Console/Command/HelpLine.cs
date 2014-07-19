using System;

namespace Tharga.Toolkit.Console.Command
{
    public class HelpLine
    {
        private readonly string _text;
        private readonly Func<bool> _canExecute;

        public string Text { get { return _text; } }
        public Func<bool> CanExecute { get { return _canExecute; } }

        public HelpLine(string text, Func<bool> canExecute)
        {
            _text = text;
            _canExecute = canExecute;
        }
    }
}