// 
// AboutDialogAction.cs
//  
// Author:
//       Jonathan Pobst <monkey@jpobst.com>
// 
// Copyright (c) 2010 Jonathan Pobst
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Text;
using System.Xml.Linq;
using Gtk;
using Pinta.Core;

namespace Pinta.Actions
{
	class AboutDialogAction : IActionHandler
	{
		#region IActionHandler Members
		public void Initialize ()
		{
			PintaCore.Actions.App.About.Activated += Activated;
		}

		public void Uninitialize ()
		{
			PintaCore.Actions.App.About.Activated -= Activated;
		}
		#endregion

		private void Activated (object sender, EventArgs e)
		{
			// TODO-GTK4 use Adw.AboutWindow after upgrading to the next gir.core release (isn't wrapped in the current version).
			var dialog = Gtk.AboutDialog.New ();
			dialog.TransientFor = PintaCore.Chrome.MainWindow;
			dialog.Title = Translations.GetString ("About Pinta");
			dialog.IconName = Pinta.Resources.Icons.AboutPinta;
			dialog.ProgramName = Translations.GetString ("Pinta");
			dialog.LogoIconName = Pinta.Resources.Icons.Pinta;
			dialog.Version = PintaCore.ApplicationVersion;
			dialog.Website = "https://www.pinta-project.com";
			dialog.Comments = Translations.GetString ("Easily create and edit images");
			dialog.Copyright = BuildCopyrightText ();
			dialog.License = BuildLicenseText ();
			dialog.Authors = authors;
			dialog.TranslatorCredits = Translations.GetString ("translator-credits");

			dialog.Present ();
		}

		private string BuildCopyrightText ()
		{
			return string.Format ("{0} (c) 2010-2023 {1}",
				Translations.GetString ("Copyright"),
				Translations.GetString ("by Pinta contributors"));
		}

		private string BuildLicenseText ()
		{
			var sb = new StringBuilder ();

			sb.AppendFormat ("{0}:\n", Translations.GetString ("License"));
			sb.AppendLine (Translations.GetString ("Released under the MIT X11 License."));
			sb.AppendLine ();

			sb.AppendLine (Translations.GetString ("Based on the work of Paint.NET:"));
			sb.AppendLine ("http://www.getpaint.net/");
			sb.AppendLine ();

			sb.AppendLine (Translations.GetString ("Using some icons from:"));
			sb.AppendLine ("Silk - http://www.famfamfam.com/lab/icons/silk");
			sb.AppendLine ("Fugue - http://pinvoke.com/");
			sb.AppendLine ("Google Material Icons - https://github.com/google/material-design-icons");
			sb.AppendLine ("Microsoft Fluent UI System Icons - https://github.com/microsoft/fluentui-system-icons");

			return sb.ToString ();
		}

		// AddCreditSection() isn't wrapped correctly by GtkSharp, so current and old authors are merged for now.
		private readonly string[] authors = new[] {
			"Cameron White (@cameronwhite)",
			"Jonathan Pobst (@jpobst)",
			"James Carroll (@MrCarroll)",
			"Bartosz Głowacki (@Zeti123)",
			"@JanDeDinoMan",
			"@supershadoe",
			"@Zekiah-A",

			// Old authors.
			"A. Karon @akaro2424",
			"Aaron Bockover",
			"Adam Doppelt",
			"Adolfo Jayme Barrientos",
			"Akshara Proddatoori",
			"Alberto Fanjul (@albfan)",
			"Anirudh Sanjeev",
			"Andrija Rajter (@rajter)",
			"André Veríssimo (@averissimo)",
			"Andrew Davis",
			"Balló György (@City-busz)",
			"Cameron White (@cameronwhite)",
			"Ciprian Mustiata",
			"Dan Dascalescu (@dandv)",
			"David Nabraczky",
			"Don McComb (@don-mccomb)",
			"Elvis Alistar",
			"Felix Schmutz",
			"Greg Lowe",
			"Hanh Pham",
			"James Gifford",
			"Jami Kettunen (@JamiKettunen)",
			"Jared Kells (@jkells)",
			"Jean-Michel Bea",
			"Jennifer Nguyen (@jeneira94)",
			"Jeremy Burns (@jaburns)",
			"Joe Hillenbrand",
			"John Burak",
			"Jon Rimmer",
			"Jonathan Bergknoff",
			"Jonathan Pobst (@jpobst)",
			"Juergen Obernolte",
			"Julian Ospald (@hasufell)",
			"Khairuddin Ni'am",
			"Krzysztof Marecki",
			"Maia Kozheva",
			"Manish Sinha",
			"Marco Rolappe",
			"Marius Ungureanu",
			"Martin Geier",
			"Mathias Fussenegger",
			"Matthias Mailänder (@Mailaender)",
			"Miguel Fazenda (@miguelfazenda)",
			"Mikhail Makarov",
			"Mykola Franchuk (@thekolian1996)",
			"Obinou Conseil",
			"Olivier Dufour",
			"Richard Cohn",
			"Robert Nordan (@robpvn)",
			"Romain Racamier (@Shuunen)",
			"Stefan Moebius (@codeprof)",
			"Timon de Groot (@tdgroot)",
			"Tom Kadwill",
			"@aivel",
			"@anadvu",
			"@darkdragon-001",
			"@jefetienne",
			"@iangzh",
			"@nikita-yfh",
			"@pikachuiscool2",
			"@scx",
			"@skkestrel",
			"@tdaffin",
			"@yaminb",
		};
	}
}
