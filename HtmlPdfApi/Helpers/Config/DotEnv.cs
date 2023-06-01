using System;
using System.IO;

namespace HtmlPdfApi.Helpers.Config
{
    public static class DotEnv
    {
        public const string DEFAULT_ENVFILENAME = ".env";

        public static void Load(string path = null, LoadOptions options = null)
        {
            if (options == null) options = LoadOptions.DEFAULT;

            var file = Path.GetFileName(path);
            if (file == null || file == string.Empty) file = DEFAULT_ENVFILENAME;
            var dir = Path.GetDirectoryName(path);
            if (dir == null || dir == string.Empty) dir = Directory.GetCurrentDirectory();
            path = Path.Combine(dir, file);

            if (options.OnlyExactPath)
            {
                if (!File.Exists(path)) path = null;
            }
            else
            {
                while (!File.Exists(path))
                {
                    var parent = Directory.GetParent(dir);
                    if (parent == null)
                    {
                        path = null;
                        break;
                    }
                    dir = parent.FullName;
                    path = Path.Combine(dir, file);
                }
            }

            if (path == null)
            {
                return;
            }

            foreach (var line in File.ReadAllLines(path))
            {
                var parts = line.Split(
                    '=',
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    continue;

                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }
    }
}