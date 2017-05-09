using log4net.Core;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Log4Net
{
    internal static class LevelConverter
    {
        public static OutputLevel ToLevel(this Level level)
        {
            if (level <= Level.Debug)
                return OutputLevel.Default;

            if (level <= Level.Info)
                return OutputLevel.Information;

            if (level <= Level.Warn)
                return OutputLevel.Warning;

            return OutputLevel.Error;
        }
    }
}