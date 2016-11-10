using System;
using System.Collections.Generic;
using System.Linq;

namespace Fity.Utils.Interpolation
{
    public class LinearInterpolater : IInterpolator
    {
        private const int numPoints = 2;

        private readonly IList<InterpolationDatapoint> dataset;

        public LinearInterpolater(IEnumerable<InterpolationDatapoint> dataset)
        {
            if (dataset.Count() < numPoints)
            {
                throw new ArgumentException("Not enough data to interpolate based on numPoints configuration", nameof(dataset));
            }

            this.dataset = dataset.OrderBy(dp => dp.Time).ToList();
        }

        public double GetAtTime(DateTime time)
        {
            if (numPoints != 2)
            {
                throw new NotSupportedException("GetAtTime() not implemented for numPoints != 2");
            }

            InterpolationDatapoint point1, point2;
            this.GetInterpolationReferencePoints(time, out point1, out point2);

            return point1.Value + (point2.Value - point1.Value) * ((time - point1.Time).TotalMilliseconds / (point2.Time - point1.Time).TotalMilliseconds);
        }

        private void GetInterpolationReferencePoints(DateTime time, out InterpolationDatapoint point1, out InterpolationDatapoint point2)
        {
            // get closest 2 points
            if (time < dataset.Min(dp => dp.Time))
            {
                point1 = dataset.First();
                point2 = dataset.Skip(1).First();
            }
            else if (time > dataset.Max(dp => dp.Time))
            {
                point1 = dataset.Reverse().Skip(1).First();
                point2 = dataset.Reverse().First();
            }
            else
            {
                point1 = null;
                point2 = null;
                for (int i = 0; i < dataset.Count; i++)
                {
                    if (dataset[i].Time >= time)
                    {
                        point1 = dataset[i - 1];
                        point2 = dataset[i];
                        break;
                    }
                }
                if (point1 == null || point2 == null)
                {
                    throw new Exception("Couldn't find reference points");
                }
            }
        }
    }
}