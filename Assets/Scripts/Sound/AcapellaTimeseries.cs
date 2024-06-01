using System;
using System.Collections.Generic;

namespace Slay
{
    [Serializable]
    public class AcapellaTimeseries
    {
        public float LengthSeconds;
        public List<AcapellaTimeseriesPoint> Points;

        public AcapellaTimeseriesPoint GetPointAtTime(float time)
        {
            AcapellaTimeseriesPoint point = null;
            if (time >= 0 && time <= LengthSeconds)
            {
                float percentComplete = time / LengthSeconds;
                point = Points[(int)(Points.Count * percentComplete)];
            }
            return point;
        }
    }

    [Serializable]
    public class AcapellaTimeseriesPoint
    {
        public float Time;
        public int Note;
        public float Frequency;
        public bool IsSilence;
    }
}
