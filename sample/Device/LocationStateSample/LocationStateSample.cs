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

/**
 * LocationStateSample
**/
public partial class LocationStateSample
{
	private static GraphicsContext graphics;
	private static bool isStarted = false;
	private static bool isEnable = false;

	private static int loopCount = 0;
	private static bool loop = true;

	private const string latitudeNoDataStr = "--.--------";
	private const string longitudeNoDataStr = "---.-------";
	private const string altitudeNoDataStr = "---- m";
	private const string speedNoDataStr = "--- km/h";
	private const string datetimeNoDataStr = "--- --, ---- --:--:--";
	private const string deviceNoDataStr = "---";
	private const string accuracyNoDataStr = "----- m";
	private const string bearingNoDataStr = "--- °";
	private static string latitudeStr = latitudeNoDataStr;
	private static string longitudeStr = longitudeNoDataStr;
	private static string altitudeStr = altitudeNoDataStr;
	private static string speedStr = speedNoDataStr;
	private static string datetimeStr = datetimeNoDataStr;
	private static string deviceStr = deviceNoDataStr;
	private static string accuracyStr = accuracyNoDataStr;
	private static string bearingStr = bearingNoDataStr;

	public static void Main (string[] args)
	{
		Initialize ();

		while (loop) {
			SystemEvents.CheckEvents ();
			Update ();
			Render ();
		}

		Terminate ();
	}

	public static void Initialize ()
	{
		// Set up the graphics system
		graphics = new GraphicsContext ();

		AdjustScreen(graphics);

		isStarted = Location.Start();
	}

	public static void Update ()
	{
		SampleDraw.Update();

		// touch data check
		List<TouchData> touchDataList = Touch.GetData(0);

		if (Location.GetEnableDevices() == LocationDeviceType.None)
		{
			isEnable = false;
		}
		else
		{
			isEnable = true;
			if ((loopCount++ % 30) == 0)
			{
				GetLocationData();
			}
		}
	}

	public static void Render ()
	{
		// Clear the screen
		graphics.SetClearColor (0.0f, 0.0f, 0.0f, 0.0f);
		graphics.Clear();

		if(isStarted == false)
		{
			MessageRender("Start failed.");
		}else{
			if(isEnable == false)
			{
				MessageRender ("No enable devices.");
			}
			else
			{
				LocationRender();
			}
		}
		// Present the screen
		graphics.SwapBuffers ();
	}

	private static void GetLocationData()
	{
		var locationData = Location.GetData();

		// Latitude
		if (locationData.HasLatitude == true)
		{
			double latitude = locationData.Latitude;
			string latitudeDirectionStr = "N";

			if(latitude < 0)
			{
				latitude *= -1.0;
				latitudeDirectionStr = "S";
			}
			latitudeStr = latitude.ToString() + " " + latitudeDirectionStr;
		}else{
			latitudeStr = latitudeNoDataStr;
		}

		// Longitude
		if (locationData.HasLongitude == true)
		{
			double longitude = locationData.Longitude;
			String longitudeDirectionStr = "E";
			
			if(longitude < 0)
			{
				longitude *= -1.0;
				longitudeDirectionStr = "W";
			}
			longitudeStr = longitude.ToString() + " " + longitudeDirectionStr;;
		}else{
			longitudeStr = longitudeNoDataStr;
		}

		// Altitude
		if (locationData.HasAltitude == true)
		{
			altitudeStr = String.Format("{0:f1} ", locationData.Altitude) + meter;
		}else{
			altitudeStr = altitudeNoDataStr;
		}

		// Speed
		double speed = locationData.Speed;
		speed *= meterPerSecoudToKmPerHouer;
			if (locationData.HasSpeed == true)
		{
			speedStr = String.Format("{0:f1} ", speed) + kiloPerHour;
		}else{
			speedStr = speedNoDataStr;
		}

		// Time
		if (locationData.HasTime == true)
		{
			long locationDataTimeUtc = locationData.Time;
			DateTime locationDataUpdateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			locationDataUpdateTime = locationDataUpdateTime.AddMilliseconds(locationDataTimeUtc);

			datetimeStr = locationDataUpdateTime.ToString("MMM. dd, yyyy") + " " + locationDataUpdateTime.ToString("HH:mm:ss") + " " + utcString;
		}else{
			datetimeStr = datetimeNoDataStr;
		}

		// enable devices
		deviceStr = locationData.DeviceType.ToString();

		// Accuracy
		if (locationData.HasAccuracy == true)
		{
			accuracyStr = String.Format("{0} ", locationData.Accuracy) + meter;
		}else{
			accuracyStr = accuracyNoDataStr;
		}
		
		// Bearing
		if (locationData.HasBearing == true)
		{
			bearingStr = String.Format ("{0:f0} ", locationData.Bearing) + degree;
		}else{
			bearingStr = bearingNoDataStr;
		}
	}

	public static void Terminate ()
	{
		Location.Stop();

		SampleDraw.Term();
		graphics.Dispose();
	}
}

} // Sample
