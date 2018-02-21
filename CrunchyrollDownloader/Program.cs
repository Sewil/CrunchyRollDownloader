using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace CrunchyrollDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Contains("-url"))
            {
                throw new Exception("Missing argument 'url'");
            }
            if (!args.Contains("-user"))
            {
                throw new Exception("Missing argument 'user'");
            }
            if (!args.Contains("-pw"))
            {
                throw new Exception("Missing argument 'pw'");
            }
            if (!args.Contains("-o"))
            {
                throw new Exception("Missing argument 'o'");
            }
            if (!ExistsOnPath("youtube-dl.exe"))
            {
                Console.WriteLine("'youtube-dl' not found in PATH. Press D to download.");
                if (Console.ReadKey().Key == ConsoleKey.D)
                {
                    string directory = @"C:\Program Files (x86)\youtube-dl\";
                    Directory.CreateDirectory(directory);
                    Download("https://yt-dl.org/downloads/latest/youtube-dl.exe", directory + "youtube-dl.exe");
                    AddToPath(directory);
                }
                else
                {
                    Environment.Exit(1);
                }
            }
            if (!ExistsOnPath("rtmpdump.exe"))
            {
                Console.WriteLine("'rtmpdump' not found in PATH. Press D to download.");
                if (Console.ReadKey().Key == ConsoleKey.D)
                {
                    string directory = @"C:\Program Files (x86)\rtmpdump-2.3\";
                    Directory.CreateDirectory(directory);
                    Download("http://rtmpdump.mplayerhq.hu/download/rtmpdump-2.3-windows.zip", directory + "rtmpdump-2.3-windows.zip");
                    ZipFile.ExtractToDirectory(directory + "rtmpdump-2.3-windows.zip",  @"C:\Program Files (x86)\");
                    AddToPath(directory);
                }
                else
                {
                    Environment.Exit(1);
                }
            }
            bool subonly = false;
            string url = null;
            string user = null;
            string pw = null;
            string height = "720p";
            string o = null;
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.ToLower() == "-subonly")
                {
                    subonly = true;
                }
                else if (arg.ToLower() == "-url")
                {
                    url = args[i + 1];
                }
                else if (arg.ToLower() == "-user")
                {
                    user = args[i + 1];
                }
                else if (arg.ToLower() == "-pw")
                {
                    pw = args[i + 1];
                }
                else if (arg.ToLower() == "-height")
                {
                    height = args[i + 1];
                }
                else if (arg.ToLower() == "-o")
                {
                    o = args[i + 1];
                }
            }
            string arguments = "-u " + user + " -p " + pw;
            if (subonly)
            {
                arguments += " --skip-download --write-sub --sub-format srt --sub-lang enUS";
            }
            else
            {
                arguments += " -f \"[height=" + height + "]\"";
            }
            arguments += " -v " + url + "-o " + o + "";
            Process.Start("youtube-dl", arguments);
        }
        private static void Download(string url, string path)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, path);
            }
        }
        private static bool ExistsOnPath(string executable)
        {
            return GetPathVariable().Split(';').Any(i => File.Exists(Path.Combine(i, executable)));
        }
        private static string GetPathVariable()
        {
            return Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
        }
        private static void AddToPath(string path)
        {
            string pathvar = GetPathVariable();
            string semicolon = pathvar.EndsWith(";") ? "" : ";";
            Environment.SetEnvironmentVariable("PATH", pathvar + semicolon + path, EnvironmentVariableTarget.Machine);
        }
    }
}
