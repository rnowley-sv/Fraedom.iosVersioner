using Mono.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml;

namespace iOSVersioner
{
	class MainClass
	{
		public static int Main (string[] args)
		{
			bool showHelp = false;
			string plistFile = "";
			string versionNumber = String.Empty;

			var optionSet = new OptionSet () {
				{ "f|file=", "The info.plist file that you want to modify the version for.", v => plistFile = v },
				{ "v|version=", "The minor version number to use, this must be an integer.", v => versionNumber = v },
				{ "h|help", "Show usage instructions for the command.", v => showHelp = v != null },
			};

			try {
				optionSet.Parse(args);
			}
			catch(OptionException e) {
				Console.Write ("iosversioner: ");
				Console.WriteLine (e.Message);
				Console.WriteLine ("Try `iosversioner --help' for more information.");
			}

			if (showHelp) {
				ShowHelp (optionSet);
				return 0;
			}

			try{
				ChangeBuildVersion (plistFile, versionNumber);
			}
			catch(ArgumentException e) {
				Console.WriteLine (e.Message);
				return 1;
			}

			return 0;
				
		}

		static void ChangeBuildVersion(string plistFile, string versionNumber) {

			if(string.IsNullOrEmpty(plistFile)) {
				throw new ArgumentException ("Invalid file name.", "plistFile");
			}

			int value;

			if (!int.TryParse (versionNumber, out value)) {
				throw new ArgumentException ("Must be an integer.", "versionNumber");
			}

			XDocument xdoc = XDocument.Load (plistFile);

			XNode child = 
				(from el in xdoc.Descendants ()
					where el.Name == "key" && el.Value == "CFBundleShortVersionString"
					select el.NextNode).Single ();

			var version = ((XElement)child).Value;
			((XElement)child).SetValue (String.Format("{0}{1}", version.Substring(0, version.LastIndexOf(".") + 1), versionNumber));

			xdoc.Save (plistFile);
		}

		static void ShowHelp(OptionSet p) {
			Console.WriteLine ("Usage: iosversioner [OPTIONS]+");
			Console.WriteLine ("Changes the CFBundleShortVersionString version to the speficied version.");
			Console.WriteLine ();
			Console.WriteLine ("Options:");
			p.WriteOptionDescriptions (Console.Out);
		}
	}
}
