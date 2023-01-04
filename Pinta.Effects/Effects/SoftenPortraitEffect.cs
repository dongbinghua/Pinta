/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Krzystzof Marecki                                       //
/////////////////////////////////////////////////////////////////////////////////

// This effect was graciously provided by David Issel, aka BoltBait. His original
// copyright and license (MIT License) are reproduced below.

/*
PortraitEffect.cs 
Copyright (c) 2007 David Issel 
Contact Info: BoltBait@hotmail.com http://www.BoltBait.com 

Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions: 

The above copyright notice and this permission notice shall be included in 
all copies or substantial portions of the Software. 

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
THE SOFTWARE. 
*/
using System;
using System.Security.Cryptography;
using Cairo;
using Pinta.Core;
using Pinta.Effects;
using Pinta.Gui.Widgets;

namespace Pinta.Effects
{
	public class SoftenPortraitEffect : BaseEffect
	{
		private GaussianBlurEffect blurEffect;
		private BrightnessContrastEffect bacAdjustment;
		private UnaryPixelOps.Desaturate desaturateOp;
		private UserBlendOps.OverlayBlendOp overlayOp;

		public override string Icon {
			get { return "Menu.Effects.Photo.SoftenPortrait.png"; }
		}

		public override string Name {
			get { return Translations.GetString ("Soften Portrait"); }
		}

		public override bool IsConfigurable {
			get { return true; }
		}

		public override string EffectMenuCategory {
			get { return Translations.GetString ("Photo"); }
		}

		public SoftenPortraitData Data { get { return (SoftenPortraitData) EffectData!; } } // NRT - Set in constructor

		public SoftenPortraitEffect ()
		{
			EffectData = new SoftenPortraitData ();

			blurEffect = new GaussianBlurEffect ();
			bacAdjustment = new BrightnessContrastEffect ();
			desaturateOp = new UnaryPixelOps.Desaturate ();
			overlayOp = new UserBlendOps.OverlayBlendOp ();
		}

		public override bool LaunchConfiguration ()
		{
			return EffectHelper.LaunchSimpleEffectDialog (this);
		}

		public override void Render (ImageSurface src, ImageSurface dest, Core.RectangleI[] rois)
		{
			int warmth = Data.Warmth;
			float redAdjust = 1.0f + (warmth / 100.0f);
			float blueAdjust = 1.0f - (warmth / 100.0f);

			this.blurEffect.Render (src, dest, rois);
			this.bacAdjustment.Render (src, dest, rois);

			ReadOnlySpan<ColorBgra> src_data = src.GetReadOnlyData ();
			Span<ColorBgra> dst_data = dest.GetData ();
			int width = dest.Width;

			foreach (var roi in rois) {
				for (int y = roi.Top; y <= roi.Bottom; ++y) {
					var src_row = src_data.Slice (y * width, width);
					var dst_row = dst_data.Slice (y * width, width);

					for (int x = roi.Left; x <= roi.Right; ++x) {
						ColorBgra srcGrey = this.desaturateOp.Apply (src_row[x]);

						srcGrey.R = Utility.ClampToByte ((int) ((float) srcGrey.R * redAdjust));
						srcGrey.B = Utility.ClampToByte ((int) ((float) srcGrey.B * blueAdjust));

						dst_row[x] = this.overlayOp.Apply (srcGrey, dst_row[x]);
					}
				}
			}
		}
	}

	public class SoftenPortraitData : EffectData
	{
		[Caption ("Softness"), MinimumValue (0), MaximumValue (10)]
		public int Softness = 5;

		[Caption ("Lighting"), MinimumValue (-20), MaximumValue (20)]
		public int Lighting = 0;

		[Caption ("Warmth"), MinimumValue (0), MaximumValue (20)]
		public int Warmth = 10;
	}
}
