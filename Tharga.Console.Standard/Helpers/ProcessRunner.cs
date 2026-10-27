using System.Diagnostics;

namespace Tharga.Console.Helpers
{
    internal static class ProcessRunner
    {
        public static string Run(string fileName, string arguments)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.Arguments = arguments;
                process.Start();

                process.StandardInput.Close();

                // Read before waiting: a child that fills the output pipe blocks until it is read.
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return output;
            }
        }
    }
}
