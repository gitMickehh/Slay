using UnityEngine;

namespace Slay
{
    [CreateAssetMenu(
        fileName = "Pitch Estimator Configuration",
        menuName = "ScriptableObjects/Pitch Estimator Configuration"
    )]
    public class PitchEstimatorConfiguration : ScriptableObject
    {
        [Tooltip("Minimum Frequency [Hz]")]
        [Range(40, 150)]
        public int FrequencyMin = 40;

        [Tooltip("Maximum Frequency [Hz]")]
        [Range(300, 1200)]
        public int FrequencyMax = 600;

        [Tooltip("Number of overtones used for estimation")]
        [Range(1, 8)]
        public int HarmonicsToUse = 5;

        [Tooltip(
            "Spectrum moving average bandwidth [Hz]. The larger the width, the smoother it is, but the accuracy is lower."
        )]
        public float SmoothingWidth = 500;

        [Tooltip(
            "Threshold value for voiced sound judgment. The larger the value, the stricter the judgment."
        )]
        public float ThresholdSRH = 7;
    }
}
