/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Device;
using Sce.PlayStation.Core.Imaging;

namespace Sample
{

	partial class LocationStateSample
	{
		private	static int lineCnt = 8;
		private static int lineHeight = 80;
		private static int fontSize = 20;
		private static int horizonalCenter = 400;
		private static int messagePosX = 0;
		private static int messagePosY = 0;
		private const double meterPerSecoudToKmPerHouer = 3.6;
		private const string meter = "m";
		private const string kiloPerHour = "km/h";
		private const string utcString = "(UTC)";
		private const string degree = "°";
		private const uint textColor = 0xffffffff;

		private static void AdjustScreen(GraphicsContext graphics)
		{
			SampleDraw.Init(graphics);
			lineHeight = graphics.Screen.Height / lineCnt;
			fontSize = lineHeight / 2;
			horizonalCenter = graphics.Screen.Width / 2;
			int quaterOffsetX = (horizonalCenter - 16) / 2;
			int maxFontSize = (graphics.Screen.Height / 20);

			if(fontSize > maxFontSize)
			{
				fontSize = maxFontSize;
			}
			var font = new Font(FontAlias.System, fontSize, FontStyle.Regular);
			SampleDraw.SetFont(font);
		}

		private static void LocationRender()
		{
			// for adjust draw position
			int verticalPosition = lineHeight;
			int coronAlignMentOffset = fontSize * 6;
			int coronPositionX = 16 + coronAlignMentOffset;

			// location data draw - left
			SampleDraw.DrawText("Latitude:",	textColor, 16, verticalPosition);
			SampleDraw.DrawText(latitudeStr,	textColor, coronPositionX, verticalPosition);
			verticalPosition += lineHeight;
			SampleDraw.DrawText("Longitude:",	textColor, 16, verticalPosition);
			SampleDraw.DrawText(longitudeStr,	textColor, coronPositionX, verticalPosition);
			verticalPosition += lineHeight;
			SampleDraw.DrawText("Altitude:",	textColor, 16, verticalPosition);
			SampleDraw.DrawText(altitudeStr,	textColor, coronPositionX, verticalPosition);
			verticalPosition += lineHeight;
			SampleDraw.DrawText("Speed:",		textColor, 16, verticalPosition);
			SampleDraw.DrawText(speedStr,		textColor, coronPositionX, verticalPosition);
			verticalPosition += lineHeight;
			SampleDraw.DrawText("Received Date & Time:",	textColor, 16, verticalPosition);
			SampleDraw.DrawText(datetimeStr, 	textColor, coronPositionX + fontSize * 5, verticalPosition);

			// location data draw - right
			verticalPosition = lineHeight;
			coronAlignMentOffset = fontSize * 6;
			coronPositionX = horizonalCenter + 10 + coronAlignMentOffset;
			SampleDraw.DrawText("Device:",		textColor, horizonalCenter, verticalPosition);
			SampleDraw.DrawText(deviceStr,		textColor, coronPositionX, verticalPosition);
			verticalPosition += lineHeight;
			SampleDraw.DrawText("Accuracy:",	textColor, horizonalCenter, verticalPosition);
			SampleDraw.DrawText(accuracyStr,	textColor, coronPositionX, verticalPosition);
			verticalPosition += lineHeight;
			SampleDraw.DrawText("Direction:",	textColor, horizonalCenter, verticalPosition);
			SampleDraw.DrawText(bearingStr,		textColor, coronPositionX, verticalPosition);
		}

		private static void MessageRender(string msg)
		{
			SampleDraw.DrawText (msg, textColor, messagePosX, messagePosY);
		}
		
	}

} // namespace Sample

