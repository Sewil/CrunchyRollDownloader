using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchyRollDownloader {
	static class StringExtension {
		public static bool ExistsOnPath(this string exeName) {
			try {
				return File.Exists(exeName.GetFullPath() ?? "");
			} catch (Win32Exception) {
				return false;
			}
		}
		public static string GetFullPath(this string exeName) {
			try {
				Process p = new Process();
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.FileName = "where";
				p.StartInfo.Arguments = exeName;
				p.StartInfo.RedirectStandardOutput = true;
				p.Start();
				string output = p.StandardOutput.ReadToEnd();
				p.WaitForExit();

				if (p.ExitCode != 0) {
					return null;
				}

				return output.Substring(0, output.IndexOf(Environment.NewLine));
			} catch (Win32Exception) {
				throw new Exception("'where' command is not on path");
			}
		}
	}
}