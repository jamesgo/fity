using System;

namespace Fity.Utils.Interpolation
{
    public interface IInterpolator
    {
        double GetAtTime(DateTime time);
    }
}