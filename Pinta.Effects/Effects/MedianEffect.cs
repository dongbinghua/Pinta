/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Hanh Pham <hanh.pham@gmx.com>                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using Cairo;
using Pinta.Core;
using Pinta.Gui.Widgets;

namespace Pinta.Effects
{
	public class MedianEffect : LocalHistogramEffect
	{
		private int radius;
		private int percentile;

		public override string Icon {
			get { return "Menu.Effects.Noise.Median.png"; }
		}

		public override string Name {
			get { return Translations.GetString ("Median"); }
		}

		public override bool IsConfigurable {
			get { return true; }
		}

		public override string EffectMenuCategory {
			get { return Translations.GetString ("Noise"); }
		}

		public MedianData Data { get { return (MedianData) EffectData!; } } // NRT - Set in constructor

		public MedianEffect ()
		{
			EffectData = new MedianData ();
		}

		public override bool LaunchConfiguration ()
		{
			return EffectHelper.LaunchSimpleEffectDialog (this);
		}

		#region Algorithm Code Ported From PDN
		public override void Render (ImageSurface src, ImageSurface dest, Core.RectangleI[] rois)
		{
			this.radius = Data.Radius;
			this.percentile = Data.Percentile;

			foreach (Core.RectangleI rect in rois)
				RenderRect (this.radius, src, dest, rect);
		}

		public override ColorBgra Apply (in ColorBgra src, int area, Span<int> hb, Span<int> hg, Span<int> hr, Span<int> ha)
		{
			ColorBgra c = GetPercentile (this.percentile, area, hb, hg, hr, ha);
			return c;
		}
		#endregion

		public class MedianData : EffectData
		{
			[Caption ("Radius"), MinimumValue (1), MaximumValue (200)]
			public int Radius = 10;

			[Caption ("Percentile"), MinimumValue (0), MaximumValue (100)]
			public int Percentile = 50;

			[Skip]
			public override bool IsDefault { get { return Radius == 0; } }
		}
	}
}
