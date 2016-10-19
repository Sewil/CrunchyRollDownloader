using System;
using System.Diagnostics;
using System.IO;
using Useful.Stuff;

namespace CrunchyRollDownloader {
	class Program {
		static void Main(string[] args) {
			if (args.Length == 0) {
				Console.WriteLine("Usage: CrunchyRollDownloader.exe [OPTIONS]");
				Console.WriteLine();
				Console.WriteLine("Options:");
				Console.WriteLine(" -u [ARG]:\tDownload URL.");
				Console.WriteLine(" -b [ARG]:\tPath to text file with one URL per row.");
				Console.WriteLine(" -s:\tExtract subtitle file only.");
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine("Dependencies:");
				Console.WriteLine(" youtube-dl: https://github.com/rg3/youtube-dl/");
				Environment.Exit(0);
			}

			string[] urls = new string[] { };
			if (User.Default.u.Length == 0) {
				Console.Write("Enter username: ");
				User.Default.u = Console.ReadLine();
			}
			if (User.Default.p.Length == 0) {
				Console.Write("Enter password: ");
				User.Default.p = Console.ReadLine();
			}
			User.Default.Save();
			string user = User.Default.u, password = User.Default.p, format = User.Default.f;
			bool subonly = false;
			for (int i = 0; i < args.Length; i++) {
				string arg = args[i];
				if (arg.ToLower() == "-s") {
					subonly = true;
				} else if (arg.ToLower() == "-u") {
					urls = new string[] { args[i + 1] };
				} else if (arg.ToLower() == "-b") {
					if (File.Exists(args[i + 1])) {
						urls = File.ReadAllLines(args[i + 1]);
					}
				}
			}

			string arguments = $"-u {user} -p {password}";
			if (subonly) {
				arguments += " --skip-download --write-sub --sub-format srt --sub-lang enUS";
			} else {
				arguments += $" -f \"{format}\"";
			}

			foreach (string url in urls) {
				string n = url.Substring(45, url.IndexOf('-', 45) - 45);
				string filePath = $"{Directory.GetCurrentDirectory()}\\{n}.mp4";

				arguments += $" -v {url} -o \"{filePath}\"";
				new ProcessStartInfo("youtube-dl.exe", arguments).StartProcess($"{arguments}");

				var file = new Useful.Stuff.Path($"{filePath}", PathType.FILE);
				file.Rename($"{file.NameWithoutExtension} [{file.CRC32}]");
			}
		}
	}
}