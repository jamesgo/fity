﻿using Fity.Data;
using Fity.Data.TCX;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls.Maps;
using System;
using Fity.Utils.Interpolation;

namespace Fity.Models
{
    public class Session
    {
        private IEnumerable<Trackpoint> trackpoints => this.Activities.SelectMany(a => a.Lap?.Trackpoints).Where(tp => tp.IsValid);

        public IEnumerable<Trackpoint> TrackpointsWithPosition => trackpoints.Where(tp => tp.HasPosition).ToList();

        public IEnumerable<Trackpoint> TrackpointsWithHeartRate => trackpoints.Where(tp => tp.HasHeartRate).ToList();

        public IEnumerable<MapElement> GetMapElements()
        {
            foreach (var trackpoint in trackpoints)
            {
                if (trackpoint.Position != null)
                {
                    var icon = new MapIcon();
                    icon.Title = trackpoint.HasHeartRate ? trackpoint.HeartRate.Value.ToString() : "NoHR";

                    icon.Location = new Windows.Devices.Geolocation.Geopoint(new Windows.Devices.Geolocation.BasicGeoposition
                    {
                        Latitude = trackpoint.Position.LatitudeDegrees,
                        Longitude = trackpoint.Position.LongitudeDegrees
                    });
                    yield return icon;
                }
            }
        }

        public IEnumerable<Activity> Activities { get; set; }

        public bool HasGps => this.TrackpointsWithPosition.Any();

        public bool HasHeartRate => this.TrackpointsWithHeartRate.Any();

        public Author Author { get; internal set; }

        public Tuple<double, double, int> GetDefaultLocationWithWeights()
        {
            return new Tuple<double, double, int>(
                TrackpointsWithPosition.Average(tp => tp.Position.LatitudeDegrees),
                TrackpointsWithPosition.Average(tp => tp.Position.LongitudeDegrees),
                TrackpointsWithPosition.Count());
        }
    }
}