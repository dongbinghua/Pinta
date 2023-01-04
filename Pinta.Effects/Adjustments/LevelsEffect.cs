/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Krzysztof Marecki <marecki.krzysztof@gmail.com>         //
/////////////////////////////////////////////////////////////////////////////////

using System;
using Cairo;
using Pinta.Core;

namespace Pinta.Effects
{
	public class LevelsEffect : BaseEffect
	{
		public override string Icon {
			get { return "Menu.Adjustments.Levels.png"; }
		}

		public override string Name {
			get { return Translations.GetString ("Levels"); }
		}

		public override bool IsConfigurable {
			get { return true; }
		}

		public override string AdjustmentMenuKey {
			get { return "L"; }
		}

		public override string AdjustmentMenuKeyModifiers {
			get { return "<Primary>"; }
		}

		public LevelsData Data { get { return (LevelsData) EffectData!; } } // NRT - Set in constructor

		public LevelsEffect ()
		{
			EffectData = new LevelsData ();
		}

		public override bool LaunchConfiguration ()
		{
#if false // TODO-GTK4
			using (var dialog = new LevelsDialog (Data)) {
				dialog.Title = Name;
				dialog.Icon = PintaCore.Resources.GetIcon (Icon);

				var response = Gtk.ResponseType.None;
				while (response == Gtk.ResponseType.None) {
					response = (Gtk.ResponseType) dialog.Run ();
				}

				return response == Gtk.ResponseType.Ok;
			}
#else
			throw new NotImplementedException ();
#endif
		}

		public override void Render (ImageSurface src, ImageSurface dest, Core.Rectangle[] rois)
		{
			Data.Levels.Apply (dest, src, rois);
		}
	}

	public class LevelsData : EffectData
	{
		public UnaryPixelOps.Level Levels { get; set; }

		public LevelsData ()
		{
			Levels = new UnaryPixelOps.Level ();
		}

		public override EffectData Clone ()
		{
			return new LevelsData () { Levels = (UnaryPixelOps.Level) Levels.Clone () };
		}
	}
}
