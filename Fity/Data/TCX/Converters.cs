using Fity.Models;
using System;
using System.IO;
using System.Linq;

namespace Fity.Data.TCX
{
    public static class Converters
    {
        #region Session

        public static Tcx ToContract(this Session session, string filePath)
        {
            if (session == null)
            {
                return null;
            }

            return new Tcx
            {
                TrainingCenterDatabase = new Data.TCX.TrainingCenterDatabase
                {
                    Activities = new Data.TCX.ActivityCollection
                    {
                        Activities = session.Activities.Select(a => a.ToContract()).ToArray()
                    },
                    Author = session.Author.ToContract()
                }
            };
        }

        public static Session ToModel(this Tcx tcx, string filePath)
        {
            if (tcx == null)
            {
                return null;
            }

            return new Session(filePath)
            {
                Activities = tcx.TrainingCenterDatabase.Activities.Activities.Select(a => a.ToModel()).ToArray(),
                Author = tcx.TrainingCenterDatabase.Author.ToModel()
            };
        }

        #endregion

        #region Activity

        public static Models.Activity ToModel(this TCX.Activity activity)
        {
            if (activity == null)
            {
                return null;

            }
            return new Models.Activity
            {
                Id = activity.Id,
                Lap = activity.Lap.ToModel(),
                Sport = activity.Sport,
                Notes = activity.Notes
            };
        }

        public static TCX.Activity ToContract(this Models.Activity activity)
        {
            if (activity == null)
            {
                return null;

            }
            return new TCX.Activity
            {
                Id = activity.Id,
                Lap = activity.Lap.ToContract(),
                Sport = activity.Sport,
                Notes = activity.Notes
            };
        }

        #endregion

        #region Author

        public static Models.Author ToModel(this TCX.Author author)
        {
            if (author == null)
            {
                return null;
            }

            return new Models.Author
            {
                LangID = author.LangID,
                Name = author.Name
            };
        }

        public static TCX.Author ToContract(this Models.Author author)
        {
            if (author == null)
            {
                return null;
            }

            return new TCX.Author
            {
                LangID = author.LangID,
                Name = author.Name
            };
        }

        #endregion

        #region Lap

        public static Models.Lap ToModel(this TCX.Lap lap)
        {
            if (lap == null || !lap.StartTimeUtc.HasValue)
            {
                return null;
            }

            return new Models.Lap(lap.StartTimeUtc.Value)
            {
                AverageHeartRateBpm = lap.AverageHeartRateBpm.ToModel(),
                Calories = lap.Calories,
                DistanceMeters = lap.DistanceMeters,
                Intensity = lap.Intensity,
                MaximumHeartRateBpm = lap.MaximumHeartRateBpm.ToModel(),
                MaximumSpeed = lap.MaximumSpeed,
                TotalTimeSeconds = lap.TotalTimeSeconds,
                Trackpoints = lap.Trackpoints?.Trackpoints?.Select(tp => tp.ToModel()).ToList(),
                TriggerMethod = lap.TriggerMethod
            };
        }

        public static TCX.Lap ToContract(this Models.Lap lap)
        {
            if (lap == null)
            {
                return null;
            }

            return new TCX.Lap
            {
                AverageHeartRateBpm = lap.AverageHeartRateBpm.ToContract(),
                Calories = lap.Calories,
                DistanceMeters = lap.DistanceMeters,
                Intensity = lap.Intensity,
                MaximumHeartRateBpm = lap.MaximumHeartRateBpm.ToContract(),
                MaximumSpeed = lap.MaximumSpeed,
                StartTime = lap.StartTime.ToString(),
                TotalTimeSeconds = lap.TotalTimeSeconds,
                Trackpoints = new TrackpointCollection
                {
                    Trackpoints = lap.Trackpoints?.Select(tp => tp.ToContract()).ToArray()
                },
                TriggerMethod = lap.TriggerMethod
            };
        }

        #endregion

        #region HeartRateBpm

        public static Models.HeartRateInBeatsPerMinute ToModel(this TCX.HeartRateInBeatsPerMinute heartRate)
        {
            if (heartRate == null)
            {
                return null;
            }

            return new Models.HeartRateInBeatsPerMinute { Value = heartRate.Value };
        }

        public static TCX.HeartRateInBeatsPerMinute ToContract(this Models.HeartRateInBeatsPerMinute heartRate)
        {
            if (heartRate == null)
            {
                return null;
            }

            return new TCX.HeartRateInBeatsPerMinute { Value = heartRate.Value };
        }

        #endregion

        #region Position

        public static Models.Position ToModel(this TCX.Position position)
        {
            if (position == null)
            {
                return null;
            }

            return new Models.Position
            {
                LatitudeDegrees = position.LatitudeDegrees,
                LongitudeDegrees = position.LongitudeDegrees
            };
        }

        public static TCX.Position ToContract(this Models.Position position)
        {
            if (position == null)
            {
                return null;
            }

            return new TCX.Position
            {
                LatitudeDegrees = position.LatitudeDegrees,
                LongitudeDegrees = position.LongitudeDegrees
            };
        }


        #endregion

        #region Trackpoint

        public static Models.Trackpoint ToModel(this TCX.Trackpoint tp)
        {
            DateTime result;
            DateTime? time;
            if (DateTime.TryParse(tp.Time, out result))
            {
                time = result;
            }
            else
            {
                time = null;
            }

            return new Models.Trackpoint(time)
            {
                AltitudeMeters = tp.AltitudeMeters,
                DistanceMeters = tp.DistanceMeters,
                HeartRate = tp.HeartRateBpm.ToModel(),
                Position = tp.Position.ToModel(), 
                Speed = tp.Extensions?.TPX != null ? (double?)tp.Extensions.TPX.Speed : null
            };
        }

        public static TCX.Trackpoint ToContract(this Models.Trackpoint tp)
        {
            var position = tp.Position != null ?
                    new TCX.Position
                    {
                        LatitudeDegrees = tp.Position.LatitudeDegrees,
                        LongitudeDegrees = tp.Position.LongitudeDegrees
                    } : null;
            var heartRate = tp.HeartRate != null ? new TCX.HeartRateInBeatsPerMinute { Value = tp.HeartRate.Value } : null;

            var trackpoint = new TCX.Trackpoint
            {
                Position = position,
                HeartRateBpm = heartRate,
                Time = tp.TimeUtc.ToString(),
                Extensions = tp.Speed.HasValue ? new TrackpointExtensions
                {
                    TPX = new TpxExtensions
                    {
                         Speed = tp.Speed.Value
                    }
                } : null
            };

            if (tp.AltitudeMeters.HasValue)
            {
                trackpoint.AltitudeMeters = tp.AltitudeMeters.Value;
            }

            if (tp.DistanceMeters.HasValue)
            {
                trackpoint.DistanceMeters = tp.DistanceMeters.Value;
            }

            return trackpoint;
        }

        #endregion
    }
}
