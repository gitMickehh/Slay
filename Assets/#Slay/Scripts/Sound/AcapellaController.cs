using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Slay
{
    public class AcapellaController : MonoBehaviour
    {
        #region Inspector
        [SerializeField]
        private AudioSource m_AudioSource;

        [SerializeField]
        private PitchEstimatorConfiguration m_PitchConfiguration;

        [SerializeField]
        private SongReference m_SongReference;

        #endregion

        #region State
        private PitchEstimator m_PitchEstimator;
        #endregion

        #region Unity Lifecycle Methods
        private void Awake() => m_PitchEstimator = new();

        private void Start() => _ = SampleAcapella(destroyCancellationToken);
        #endregion

        //#region API
        //public static string SongPath => Application.streamingAssetsPath + "/Britney.json";
        //public static string SongPath => Application.streamingAssetsPath + "/Charlie.json";

        //#endregion

        #region Acapella Sampler
        private async Awaitable SampleAcapella(CancellationToken destroyToken)
        {
            string outputFile = m_SongReference.Value.SongPath;
            if (File.Exists(outputFile))
            {
                Debug.Log("Acapella timeseries file already exists");
                return;
            }
            else
            {
                Debug.Log("Acapella timeseries file in the making: " + outputFile);
            }

            m_AudioSource.clip = m_SongReference.Value.acapella;

            AcapellaTimeseries timeseries =
                new() { Points = new(), LengthSeconds = m_AudioSource.clip.length };
            m_AudioSource.Play();
            while (!destroyToken.IsCancellationRequested && m_AudioSource.isPlaying)
            {
                // Estimate Pitch
                AcapellaTimeseriesPoint point = new();
                float estimate = m_PitchEstimator.Estimate(m_AudioSource, m_PitchConfiguration);
                if (float.IsNaN(estimate))
                {
                    point.IsSilence = true;
                }
                else
                {
                    point.Time = m_AudioSource.time;
                    point.Frequency = estimate;
                    point.Note = PitchEstimator.MidiNoteFromFrequency(estimate);
                }
                timeseries.Points.Add(point);

                // Every Other Frame
                await Awaitable.NextFrameAsync();
                await Awaitable.NextFrameAsync();
            }

            // Serialize
            await File.WriteAllTextAsync(
                //Application.streamingAssetsPath + "/Britney.json",
                //Application.streamingAssetsPath + "/Charlie.json",
                m_SongReference.Value.SongPath,
                JsonUtility.ToJson(timeseries)
            );
            Debug.Log("DONE");
        }
        #endregion
    }
}
