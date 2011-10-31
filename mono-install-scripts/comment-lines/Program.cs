using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FixGnomeSharp
{
	public class Program
	{
		public static int Main(string[] args)
		{
			string fileName = null;
			string[] sLines = null;
			int[] lines = null;

			if (args != null)
			{
				foreach (var arg in args)
				{
					if (arg.StartsWith("-f:") && arg.Length > 3)
						fileName = arg.Substring(3);
					if (arg.StartsWith("-l:") && arg.Length > 3)
					{
						sLines = arg.Substring(3).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
						lines = new int[sLines.Length];
						for (int ix = 0; ix < sLines.Length; ix++)
							lines[ix] = int.Parse(sLines[ix]);
					}
				}
			}

			if (lines == null || lines.Length == 0 || fileName == null || !File.Exists(fileName))
			{
				PrintUsage();
				return 1;
			}


			/*comment line 221, 449 and 450*/
			using (var ms = new MemoryStream())
			{
				using (var writer = new StreamWriter(ms, new UTF8Encoding(false, false)))
				{
					using (var fileStream = File.OpenRead(fileName))
					{
						using (var reader = new StreamReader(fileStream, new UTF8Encoding(false, false)))
						{
							int ix = 1;
							while (!reader.EndOfStream)
							{
								var line = reader.ReadLine();
								if (lines.Contains(ix))
									writer.Write('#');

								writer.Write(line + '\n');
								ix++;
							}
						}
					}
				}
				File.WriteAllBytes(fileName, ms.ToArray());
			}
			return 0;
		}

		static void PrintUsage()
		{
			Console.WriteLine("Cannot comment without both arguments. Usage: comment-lines -f:FILE -l:LINE#[,LINE#...]");
		}
	}
}
