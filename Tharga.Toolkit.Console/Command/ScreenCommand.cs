using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    internal class ScreenCommand : ContainerCommandBase
    {
        public ScreenCommand(IConsole console)
            : base(console, new[] { "screen", "scr" }, null, true)
        {
            RegisterCommand(new ClearCommand(console));
            RegisterCommand(new BackgroundColorCommand(console));
            RegisterCommand(new ForegroundColorCommand(console));
            RegisterCommand(new MuteCommand(console));
            RegisterCommand(new UnmuteCommand(console));
        }

        //public override IEnumerable<HelpLine> HelpText { get { yield return new HelpLine(""); } }
    }
}