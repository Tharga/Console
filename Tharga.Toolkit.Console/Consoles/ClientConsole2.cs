using System.Diagnostics;
using System.IO;
using Tharga.Toolkit.Console.Consoles.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Consoles
{
    public class ClientConsole2 : ConsoleBase2
    {
        public ClientConsole2(IConsoleConfiguration consoleConfiguration = null)
            : base(new ConsoleManager2(System.Console.Out, System.Console.In))
        {
            Initiate(consoleConfiguration);
        }

        private void Initiate(IConsoleConfiguration consoleConfiguration)
        {
            //SetLocation(consoleConfiguration);
            //SetTopMost(consoleConfiguration.TopMost);
            SetColor(consoleConfiguration);
            UpdateTitle(consoleConfiguration);
            ShowSplashScreen(consoleConfiguration);
            ShowAssemblyInfo(consoleConfiguration);
        }

        private void SetColor(IConsoleConfiguration consoleConfiguration)
        {
            if (ConsoleManager.BackgroundColor == consoleConfiguration.BackgroundColor && ConsoleManager.ForegroundColor == consoleConfiguration.DefaultTextColor) return;

            ConsoleManager.BackgroundColor = consoleConfiguration.BackgroundColor;
            ConsoleManager.ForegroundColor = consoleConfiguration.DefaultTextColor;
            ConsoleManager.Clear();
        }

        private void UpdateTitle(IConsoleConfiguration consoleConfiguration)
        {
            try
            {
                ConsoleManager.Title = consoleConfiguration.Title ?? AssemblyHelper.GetAssemblyInfo() ?? "Tharga Console";
            }
            catch (IOException exception)
            {
                Trace.TraceError($"Cannot set console title. {exception.Message}");
            }
        }

        private void ShowSplashScreen(IConsoleConfiguration consoleConfiguration)
        {
            if (string.IsNullOrEmpty(consoleConfiguration.SplashScreen))
            {
                return;
            }

            Output(new WriteEventArgs(consoleConfiguration.SplashScreen));
        }

        private void ShowAssemblyInfo(IConsoleConfiguration consoleConfiguration)
        {
            if (consoleConfiguration.ShowAssemblyInfo)
            {
                var info = AssemblyHelper.GetAssemblyInfo();
                if (!string.IsNullOrEmpty(info))
                {
                    Output(new WriteEventArgs(info));
                }
            }
        }
    }
}