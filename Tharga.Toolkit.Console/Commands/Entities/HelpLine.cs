using System;

namespace Tharga.Toolkit.Console.Commands.Entities
{
    public class HelpLine
    {
        public string Text { get; }
        public Func<bool> CanExecute { get; }
        public ConsoleColor ForeColor { get; }

        public HelpLine(string text, ConsoleColor foreColor = ConsoleColor.DarkMagenta)
        {
            Text = text;
            CanExecute = null;
            ForeColor = foreColor;
        }

        public HelpLine(string text, Func<bool> canExecute, ConsoleColor foreColor)
        {
            Text = text;
            CanExecute = canExecute;
            ForeColor = foreColor;
        }
    }
}