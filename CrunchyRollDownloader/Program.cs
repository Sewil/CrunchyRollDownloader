using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using SewilLibrary;
using System.IO.Compression;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CrunchyRollDownloader {
	class Program {
		public static string AssemblyDirectory {
			get {
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return System.IO.Path.GetDirectoryName(path);
			}
		}

		static bool subonly = false;
		static string[] urls = new string[] { };
		static bool downloaded = false;
		static Useful.Stuff.Path youtubedl;
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
				Console.WriteLine("   rtmpdump: http://rtmpdump.mplayerhq.hu/download");
				Environment.Exit(0);
			}

			if (User.Default.u.Length == 0) {
				Console.Write("Enter username: ");
				User.Default.u = Console.ReadLine();
			}
			if (User.Default.p.Length == 0) {
				Console.Write("Enter password: ");
				User.Default.p = Console.ReadLine();
			}
			User.Default.Save();

			for (int i = 0; i < args.Length; i++) {
				string arg = args[i];
				if (arg.ToLower() == "-s") {
					subonly = true;
				} else if (arg.ToLower() == "-u") {
					urls = new string[] { args[i + 1] };
				} else if (arg.ToLower() == "-b") {
					if (System.IO.File.Exists(args[i + 1])) {
						urls = System.IO.File.ReadAllLines(args[i + 1]);
					}
				}
			}

			youtubedl = new Useful.Stuff.Path(AssemblyDirectory + "\\youtube-dl.exe", PathType.FILE);
			if (!"rtmpdump".ExistsOnPath()) {
				Console.WriteLine("Couldn't find rtmpdump, would you like to download it? (Y/N)");
				ConsoleKey key = new ConsoleKey();
				do {
					key = Console.ReadKey(true).Key;
				} while (key != ConsoleKey.Y && key != ConsoleKey.N);
				if (key == ConsoleKey.N) {
					Console.WriteLine("Please download rtmpdump before continuing. Exiting...");
					Environment.Exit(0);
				}

				using (var client = new WebClient()) {
					client.DownloadProgressChanged += Client_DownloadProgressChanged;
					client.DownloadFileCompleted += Client_DownloadFileCompleted;
					Directory.CreateDirectory(System.IO.Path.GetTempPath() + "\\CrunchyRollDownloader");
					client.DownloadFileAsync(new Uri("http://rtmpdump.mplayerhq.hu/download/rtmpdump-2.3-windows.zip"), System.IO.Path.GetTempPath() + "\\CrunchyRollDownloader\\rtmpdump-2.3-windows.zip");
				}

				while (!downloaded) { }

				string dir = System.IO.Path.GetTempPath() + "\\CrunchyRollDownloader";
				Useful.Stuff.Path zip = new Useful.Stuff.Path(dir + "\\rtmpdump-2.3-windows.zip", PathType.FILE);
				ZipFile.ExtractToDirectory(zip.ToString(), dir);
				Useful.Stuff.Path rtmpdump = new Useful.Stuff.Path(dir + "\\rtmpdump-2.3\\rtmpdump.exe", PathType.FILE);
				rtmpdump.Move(AssemblyDirectory + "\\" + rtmpdump.Name);
			}

			DownloadEpisodes(urls, subonly);
		}
		static void DownloadEpisodes(string[] urls, bool subonly) {
			string user = User.Default.u, password = User.Default.p, format = User.Default.f;

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
				new ProcessStartInfo(youtubedl.ToString(), arguments).StartProcess($"{arguments}");

				var file = new Useful.Stuff.Path($"{filePath}", PathType.FILE);
				file.Rename($"{file.NameWithoutExtension} [{file.CRC32}]");
			}
		}
		private static void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
			Console.Write("\rDownloading... Done.              ");
			downloaded = true;
		}

		static char[] progressChars = new char[] { '|', '/', '\\' };
		static int pCharIndex = 0;
		private static void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
			StringBuilder bar = new StringBuilder("----------");
			for (int p = 0; p <= e.ProgressPercentage; p++) {
				if (p >= 10) {
					int i = Convert.ToInt32(Math.Floor((double)(p / 10)) - 1);
					bar[i] = '#';
				}
			}
			if (pCharIndex == progressChars.Length) {
				pCharIndex = 0;
			}

			Console.Write($"\rDownloading... [{bar.ToString()}] {e.ProgressPercentage}% {progressChars[pCharIndex++]}  ");
		}
	}
}
