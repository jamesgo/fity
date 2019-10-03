using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Fity.Data;
using Windows.Storage;
using System.Threading.Tasks;
using Fity.Utils.Interpolation;
using System.Collections.Generic;

namespace UnitTests.Utils
{
    [TestClass]
    public class LinearInterpolationTests
    {
        [TestMethod]
        public void Slope1()
        {
            var utcNow = DateTime.UtcNow;

            var interpolator = new LinearInterpolater(new List<InterpolationDatapoint>
            {
                new InterpolationDatapoint { Time = utcNow, Value = 0 },
                new InterpolationDatapoint { Time = utcNow.AddMinutes(1), Value = 1 }
            });
            Assert.AreEqual(-1D, interpolator.GetAtTime(utcNow.AddMinutes(-1)));
            Assert.AreEqual(0D, interpolator.GetAtTime(utcNow));
            Assert.AreEqual(1D, interpolator.GetAtTime(utcNow.AddMinutes(1)));
            Assert.AreEqual(2D, interpolator.GetAtTime(utcNow.AddMinutes(2)));
            Assert.AreEqual(3D, interpolator.GetAtTime(utcNow.AddMinutes(3)));
        }

        [TestMethod]
        public void Slope2Offset()
        {
            var utcNow = DateTime.UtcNow;

            var interpolator = new LinearInterpolater(new List<InterpolationDatapoint>
            {
                new InterpolationDatapoint { Time = utcNow.AddMinutes(3), Value = 1 },
                new InterpolationDatapoint { Time = utcNow.AddMinutes(6), Value = 7 }
            });

            Assert.AreEqual(-5D, interpolator.GetAtTime(utcNow));
            Assert.AreEqual(-3D, interpolator.GetAtTime(utcNow.AddMinutes(1)));
            Assert.AreEqual(5D, interpolator.GetAtTime(utcNow.AddMinutes(5)));
        }
    }
}
