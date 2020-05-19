#if __ANDROID__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;

using Codon.ApplicationModel;
using Codon.ComponentModel;
using Codon.Concurrency;
using Codon.Messaging;

namespace Codon.GeoLocation
{
	public class GeoLocator : Java.Lang.Object, IGeoLocator, ILocationListener
		, IMessageSubscriber<ApplicationLifeCycleMessage>
	{
		LocationManager locationManager;
		string locationProvider;

		readonly PropertyChangeNotifier notifier;
		bool monitoring;

		public GeoLocator()
		{
			notifier = new PropertyChangeNotifier(this, false);
		}
		
		void InitializeLocationManager()
		{
			var context = Application.Context;
			locationManager = (LocationManager)context.GetSystemService(Context.LocationService);
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = desiredAccuracy.ToAccuracy()
			};

			IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);

			if (acceptableLocationProviders.Any())
			{
				locationProvider = acceptableLocationProviders.First();
			}
			else
			{
				locationProvider = string.Empty;
			}

			locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
		}

		void StopMonitoring()
		{
			locationManager.RemoveUpdates(this);

			monitoring = false;
		}

		PositionAccuracy desiredAccuracy;

		public PositionAccuracy DesiredAccuracy
		{
			get => desiredAccuracy;
			set => notifier.Set(ref desiredAccuracy, value);
		}

		PositionStatus locationStatus;

		public PositionStatus LocationStatus
		{
			get => locationStatus;
			private set => notifier.Set(ref locationStatus, value);
		}

		double movementThresholdMeters;

		public double MovementThresholdMeters
		{
			get => movementThresholdMeters;
			set => notifier.Set(ref movementThresholdMeters, value);
		}

		uint reportInterval;

		public uint ReportInterval
		{
			get => reportInterval;
			set => notifier.Set(ref reportInterval, value);
		}

		GeopositionWrapper geoposition;

		public GeopositionWrapper Geoposition
		{
			get => geoposition;
			set => notifier.Set(ref geoposition, value);
		}

		public event EventHandler<PositionChangedProxyEventArgs> PositionChanged;

		protected virtual void OnPositionChanged(PositionChangedProxyEventArgs e)
		{
			PositionChanged?.Invoke(this, e);
		}

		public event EventHandler<StatusChangedProxyEventArgs> StatusChanged;

		protected virtual void OnStatusChanged(StatusChangedProxyEventArgs e)
		{
			StatusChanged?.Invoke(this, e);
		}

		public Task<GeopositionWrapper> GetGeoCoordinateAsync()
		{
			return Task.FromResult(geoposition);
		}

		public Task<GeopositionWrapper> GetGeoCoordinateAsync(TimeSpan maximumAge, TimeSpan timeout)
		{
			return GetGeoCoordinateAsync();
		}

		public void Start()
		{
			if (monitoring)
			{
				return;
			}

			InitializeLocationManager();
		}

		public void Stop()
		{
			StopMonitoring();
		}

		public void OnLocationChanged(Location location)
		{
			var temp = new GeopositionWrapper(location);
			Geoposition = temp;

			OnPositionChanged(new PositionChangedProxyEventArgs(temp));
		}

		public void OnProviderDisabled(string provider)
		{
			
		}

		public void OnProviderEnabled(string provider)
		{
			
		}

		public void OnStatusChanged(string provider, Availability availability, Bundle extras)
		{
			Status = availability;

			OnStatusChanged(new StatusChangedProxyEventArgs(availability.ToStatus()));
		}

		Availability status;

		public Availability Status
		{
			get => status;
			private set => notifier.Set(ref status, value);
		}

		public async Task<Address> GetAddressOfLocation(Location location)
		{
			AssertArg.IsNotNull(location, nameof(location));

			var context = Application.Context;
			Geocoder geocoder = new Geocoder(context);
			IList<Address> addressList = await geocoder.GetFromLocationAsync(
													location.Latitude, location.Longitude, 10);

			Address address = addressList.FirstOrDefault();
			return address;
		}

		Task IMessageSubscriber<ApplicationLifeCycleMessage>.ReceiveMessageAsync(ApplicationLifeCycleMessage message)
		{
			if (monitoring && locationManager != null)
			{
				if (message.State == ApplicationLifeCycleState.Activated)
				{
					locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
				}
				else if (message.State == ApplicationLifeCycleState.Deactivated)
				{
					locationManager.RemoveUpdates(this);
				}
			}

			return Task.CompletedTask;
		}
	}

	public static class PositionAccuracyAndroidExtensions
	{
		public static Accuracy ToAccuracy(this PositionAccuracy positionAccuracy)
		{
			switch (positionAccuracy)
			{
				case PositionAccuracy.Coarse:
					return Accuracy.Coarse;
				case PositionAccuracy.Default:
					return Accuracy.NoRequirement;
				case PositionAccuracy.Fine:
					return Accuracy.Fine;
				case PositionAccuracy.High:
					return Accuracy.High;
				case PositionAccuracy.Low:
					return Accuracy.Low;
				case PositionAccuracy.Medium:
					return Accuracy.Medium;
			}

			throw new ArgumentOutOfRangeException(nameof(positionAccuracy), "Unknown value '" + positionAccuracy + "'");
		}
	}

	public static class PositionStatusExtensions
	{
		public static PositionStatus ToStatus(this Availability availability)
		{
			switch (availability)
			{
				case Availability.Available:
				return PositionStatus.Ready;
				case Availability.OutOfService:
				return PositionStatus.Disabled;
				case Availability.TemporarilyUnavailable:
				return PositionStatus.NoData;
			}

			throw new ArgumentOutOfRangeException(nameof(availability), "Unknown value '" + availability + "'");
		}
	}
}

#endif