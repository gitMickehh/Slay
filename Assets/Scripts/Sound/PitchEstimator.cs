using System.Runtime.CompilerServices;
using UnityEngine;

namespace Slay
{
    public class PitchEstimator
    {
        #region Constants
        public const int k_MidiNoteCount = 128;

        // Number of elements on the frequency axis of SRH
        // (reducing it will reduce the calculation load)
        private const int K_OutputResolution = 200;
        private const int k_SpectrumSize = 1024;
        private static readonly string[] k_NoteNames = new string[]
        {
            "", // 0
            "", // 1
            "", // 2
            "", // 3
            "", // 4
            "", // 5
            "", // 6
            "", // 7
            "", // 8
            "", // 9
            "", // 10
            "", // 11
            "", // 12
            "", // 13
            "", // 14
            "", // 15
            "", // 16
            "", // 17
            "", // 18
            "", // 19
            "", // 20
            "A0",
            "A#0/Bb0",
            "B0",
            "C1",
            "C#1/Db1",
            "D1",
            "D#1/Eb1",
            "E1",
            "F1",
            "F#1/Gb1",
            "G1",
            "G#1/Ab1",
            "A1",
            "A#1/Bb1",
            "B1",
            "C2",
            "C#2/Db2",
            "D2",
            "D#2/Eb2",
            "E2",
            "F2",
            "F#2/Gb2",
            "G2",
            "G#2/Ab2",
            "A2",
            "A#2/Bb2",
            "B2",
            "C3",
            "C#3/Db3",
            "D3",
            "D#3/Eb3",
            "E3",
            "F3",
            "F#3/Gb3",
            "G3",
            "G#3/Ab3",
            "A3",
            "A#3/Bb3",
            "B3",
            "C4",
            "C#4/Db4",
            "D4",
            "D#4/Eb4",
            "E4",
            "F4",
            "F#4/Gb4",
            "G4",
            "G#4/Ab4",
            "A4",
            "A#4/Bb4",
            "B4",
            "C5",
            "C#5/Db5",
            "D5",
            "D#5/Eb5",
            "E5",
            "F5",
            "F#5/Gb5",
            "G5",
            "G#5/Ab5",
            "A5",
            "A#5/Bb5",
            "B5",
            "C6",
            "C#6/Db6",
            "D6",
            "D#6/Eb6",
            "E6",
            "F6",
            "F#6/Gb6",
            "G6",
            "G#6/Ab6",
            "A6",
            "A#6/Bb6",
            "B6",
            "C7",
            "C#7/Db7",
            "D7",
            "D#7/Eb7",
            "E7",
            "F7",
            "F#7/Gb7",
            "G7",
            "G#7/Ab7",
            "A7",
            "A#7/Bb7",
            "B7",
            "C8",
            "C#8/Db8",
            "D8",
            "D#8/Eb8",
            "E8",
            "F8",
            "F#8/Gb8",
            "G8",
            "G#8/Ab8",
            "A8",
            "A#8/Bb8",
            "B8",
            "C9",
            "C#9/Db9",
            "D9",
            "D#9/Eb9",
            "E9",
            "F9",
            "F#9/Gb9",
            "G9",
        };
        #endregion

        #region State
        private float[] m_Spectrum;
        private float[] m_SpecRaw;
        private float[] m_SpecCum;
        private float[] m_SpecRes;
        private float[] m_SRH;
        // public List<float> SRH => new List<float>(srh);
        #endregion

        #region Unity Lifecycle Events
        public PitchEstimator()
        {
            m_Spectrum = new float[k_SpectrumSize];
            m_SpecRaw = new float[k_SpectrumSize];
            m_SpecCum = new float[k_SpectrumSize];
            m_SpecRes = new float[k_SpectrumSize];
            m_SRH = new float[K_OutputResolution];
        }
        #endregion

        /// <summary>
        /// Returns the midi note for a given frequency.
        /// </summary>
        /// <param name="frequency">Frequency of the note</param>
        /// <returns>The midi note number</returns>
        public static int MidiNoteFromFrequency(float frequency) =>
            Mathf.RoundToInt(12 * Mathf.Log(frequency / 440) / Mathf.Log(2) + 69);

        /// <summary>
        /// Returns the note name for a midi node.
        /// </summary>
        /// <param name="note">The midi note number</param>
        /// <returns>The name for the midi note number</returns>
        public static string MidiNoteToName(int note) => k_NoteNames[note];

        /// <summary>
        /// Estimate the fundamental frequency
        /// </summary>
        /// <param name="audioSource">input sound source</param>
        /// <returns>Fundamental frequency [Hz] (float.NaN when absent)</returns>
        public float Estimate(AudioSource audioSource, PitchEstimatorConfiguration config)
        {
            float nyquistFreq = AudioSettings.outputSampleRate / 2.0f;

            // Get audio spectrum
            if (!audioSource.isPlaying)
                return float.NaN;
            audioSource.GetSpectrumData(m_Spectrum, 0, FFTWindow.Hanning);

            // Calculate the logarithm of the amplitude spectrum
            // All subsequent spectra are treated as logarithmic amplitudes
            // (this differs from the original paper)
            for (int i = 0; i < k_SpectrumSize; i++)
            {
                // When the amplitude is zero, it becomes -∞, so add a small value.
                m_SpecRaw[i] = Mathf.Log(m_Spectrum[i] + 1e-9f);
            }

            // Cumulative sum of spectra (for later use)
            m_SpecCum[0] = 0;
            for (int i = 1; i < k_SpectrumSize; i++)
            {
                m_SpecCum[i] = m_SpecCum[i - 1] + m_SpecRaw[i];
            }

            // Calculate residual spectrum
            var halfRange = Mathf.RoundToInt(
                config.SmoothingWidth / 2 / nyquistFreq * k_SpectrumSize
            );
            for (int i = 0; i < k_SpectrumSize; i++)
            {
                // Smooth the spectrum (moving average using cumulative sum)
                int indexUpper = Mathf.Min(i + halfRange, k_SpectrumSize - 1);
                int indexLower = Mathf.Max(i - halfRange + 1, 0);
                float upper = m_SpecCum[indexUpper];
                float lower = m_SpecCum[indexLower];
                float smoothed = (upper - lower) / (indexUpper - indexLower);

                // Remove smooth components from original spectrum
                m_SpecRes[i] = m_SpecRaw[i] - smoothed;
            }

            // SRH (Summation of Residual Harmonics) calculate the score of
            float bestFreq = 0,
                bestSRH = 0;
            for (int i = 0; i < K_OutputResolution; i++)
            {
                float currentFreq =
                    (float)i
                        / (K_OutputResolution - 1)
                        * (config.FrequencyMax - config.FrequencyMin)
                    + config.FrequencyMin;

                // Calculate the SRH score at the current frequency: paper formula (1)
                float currentSRH = GetSpectrumAmplitude(m_SpecRes, currentFreq, nyquistFreq);
                for (int h = 2; h <= config.HarmonicsToUse; h++)
                {
                    // At h times the frequency, the stronger the signal the better
                    currentSRH += GetSpectrumAmplitude(m_SpecRes, currentFreq * h, nyquistFreq);

                    // At frequencies between h-1 and h times
                    // the stronger the signal, the worse it is.
                    currentSRH -= GetSpectrumAmplitude(
                        m_SpecRes,
                        currentFreq * (h - 0.5f),
                        nyquistFreq
                    );
                }
                m_SRH[i] = currentSRH;

                // Record the frequency with the highest score
                if (currentSRH > bestSRH)
                {
                    bestFreq = currentFreq;
                    bestSRH = currentSRH;
                }
            }

            // SRH score is below the threshold
            // It is assumed that there is no clear fundamental frequency
            if (bestSRH < config.ThresholdSRH)
                return float.NaN;

            return bestFreq;
        }

        // Obtain the amplitude at frequency [Hz] from spectrum data
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetSpectrumAmplitude(float[] spec, float frequency, float nyquistFreq)
        {
            float position = frequency / nyquistFreq * spec.Length;
            int index0 = (int)position;
            int index1 = index0 + 1; // Skip array bounds checking
            float delta = position - index0;
            return (1 - delta) * spec[index0] + delta * spec[index1];
        }
    }
}
