// 
// Palette.cs
//  
// Author:
//       Maia Kozheva <sikon@ubuntu.com>
// 
// Copyright (c) 2010 Maia Kozheva <sikon@ubuntu.com>
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Cairo;
using Gtk;

namespace Pinta.Core
{
	public sealed class Palette
	{
		public event EventHandler? PaletteChanged;

		private List<Color> colors;

		private Palette ()
		{
			colors = new List<Color> ();
		}

		private void OnPaletteChanged ()
		{
			if (PaletteChanged != null)
				PaletteChanged (this, EventArgs.Empty);
		}

		public int Count {
			get {
				return colors.Count;
			}
		}

		public Color this[int index] {
			get {
				return colors[index];
			}

			set {
				colors[index] = value;
				OnPaletteChanged ();
			}
		}

		public void Resize (int newSize)
		{
			int difference = newSize - Count;

			if (difference > 0) {
				for (int i = 0; i < difference; i++)
					colors.Add (new Color (1, 1, 1));
			} else {
				colors.RemoveRange (newSize, -difference);
			}

			colors.TrimExcess ();
			OnPaletteChanged ();
		}

		public static Palette GetDefault ()
		{
			Palette p = new Palette ();
			p.LoadDefault ();
			return p;
		}

		public void LoadDefault ()
		{
			colors.Clear ();

			colors.Add (new Color (255 / 255f, 255 / 255f, 255 / 255f));
			colors.Add (new Color (0 / 255f, 0 / 255f, 0 / 255f));

			colors.Add (new Color (160 / 255f, 160 / 255f, 160 / 255f));
			colors.Add (new Color (128 / 255f, 128 / 255f, 128 / 255f));

			colors.Add (new Color (64 / 255f, 64 / 255f, 64 / 255f));
			colors.Add (new Color (48 / 255f, 48 / 255f, 48 / 255f));

			colors.Add (new Color (255 / 255f, 0 / 255f, 0 / 255f));
			colors.Add (new Color (255 / 255f, 127 / 255f, 127 / 255f));

			colors.Add (new Color (255 / 255f, 106 / 255f, 0 / 255f));
			colors.Add (new Color (255 / 255f, 178 / 255f, 127 / 255f));

			colors.Add (new Color (255 / 255f, 216 / 255f, 0 / 255f));
			colors.Add (new Color (255 / 255f, 233 / 255f, 127 / 255f));

			colors.Add (new Color (182 / 255f, 255 / 255f, 0 / 255f));
			colors.Add (new Color (218 / 255f, 255 / 255f, 127 / 255f));

			colors.Add (new Color (76 / 255f, 255 / 255f, 0 / 255f));
			colors.Add (new Color (165 / 255f, 255 / 255f, 127 / 255f));

			colors.Add (new Color (0 / 255f, 255 / 255f, 33 / 255f));
			colors.Add (new Color (127 / 255f, 255 / 255f, 142 / 255f));

			colors.Add (new Color (0 / 255f, 255 / 255f, 144 / 255f));
			colors.Add (new Color (127 / 255f, 255 / 255f, 197 / 255f));

			colors.Add (new Color (0 / 255f, 255 / 255f, 255 / 255f));
			colors.Add (new Color (127 / 255f, 255 / 255f, 255 / 255f));

			colors.Add (new Color (0 / 255f, 148 / 255f, 255 / 255f));
			colors.Add (new Color (127 / 255f, 201 / 255f, 255 / 255f));

			colors.Add (new Color (0 / 255f, 38 / 255f, 255 / 255f));
			colors.Add (new Color (127 / 255f, 146 / 255f, 255 / 255f));

			colors.Add (new Color (72 / 255f, 0 / 255f, 255 / 255f));
			colors.Add (new Color (161 / 255f, 127 / 255f, 255 / 255f));

			colors.Add (new Color (178 / 255f, 0 / 255f, 255 / 255f));
			colors.Add (new Color (214 / 255f, 127 / 255f, 255 / 255f));

			colors.Add (new Color (255 / 255f, 0 / 255f, 220 / 255f));
			colors.Add (new Color (255 / 255f, 127 / 255f, 237 / 255f));

			colors.Add (new Color (255 / 255f, 0 / 255f, 110 / 255f));
			colors.Add (new Color (255 / 255f, 127 / 255f, 182 / 255f));

			colors.TrimExcess ();
			OnPaletteChanged ();
		}

		public void Load (GLib.IFile file)
		{
			List<Color>? loaded_colors = null;
			var errors = new StringBuilder ();

			var loader = PintaCore.PaletteFormats.GetFormatByFilename (file.GetDisplayName ())?.Loader;
			if (loader != null) {
				loaded_colors = loader.Load (file);
			} else {
				// Not a recognized extension, so attempt all formats
				foreach (var format in PintaCore.PaletteFormats.Formats.Where (f => !f.IsWriteOnly ())) {
					try {
						loaded_colors = format.Loader.Load (file);
						if (loaded_colors != null) {
							break;
						}
					} catch (Exception e) {
						// Record errors in case none of the formats work.
						errors.AppendLine ($"Failed to load palette as {format.Filter.Name}:");
						errors.Append (e.ToString ());
						errors.AppendLine ();
					}
				}
			}

			if (loaded_colors is not null) {
				colors = loaded_colors;
				colors.TrimExcess ();
				OnPaletteChanged ();
			} else {
				var parent = PintaCore.Chrome.MainWindow;
				ShowUnsupportedFormatDialog (parent, file.ParsedName, "Unsupported palette format", errors.ToString ());
			}
		}

		public void Save (GLib.IFile file, IPaletteSaver saver)
		{
			saver.Save (colors, file);
		}

		private void ShowUnsupportedFormatDialog (Window parent, string filename, string primaryText, string details)
		{
			string markup = "<span weight=\"bold\" size=\"larger\">{0}</span>\n\n{1}";

			string secondaryText = Translations.GetString ("Could not open file: {0}", filename);
			secondaryText += string.Format ("\n\n{0}\n", Translations.GetString ("Pinta supports the following palette formats:"));
			var extensions = from format in PintaCore.PaletteFormats.Formats
					 where format.Loader != null
					 from extension in format.Extensions
					 where char.IsLower (extension.FirstOrDefault ())
					 orderby extension
					 select extension;

			secondaryText += String.Join (", ", extensions);

			string message = string.Format (markup, primaryText, secondaryText);
			PintaCore.Chrome.ShowUnsupportedFormatDialog (parent, message, details);
		}
	}
}
