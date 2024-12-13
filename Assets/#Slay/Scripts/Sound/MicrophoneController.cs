using System.Threading;
using Techno;
using UnityEngine;

namespace Slay
{
    public class MicrophoneController : MonoBehaviour
    {
        #region Inspector
        [SerializeField]
        private AudioSource m_AudioSource;

        [SerializeField]
        private PitchEstimatorConfiguration m_PitchConfiguration;
        #endregion

        #region State
        private string m_MicrophoneName;
        private PitchEstimator m_PitchEstimator;
        private AcapellaTimeseriesPoint m_AcapellaTimeseriesPoint;
        #endregion

        #region Unity Lifecycle Events
        private void Awake()
        {
            m_PitchEstimator = new();
            m_AcapellaTimeseriesPoint = new() { IsSilence = true };
            ServiceLocator<MicrophoneController>.Service = this;
        }

        private void OnDestroy() => ServiceLocator<MicrophoneController>.Reset();

        private void Start()
        {
            foreach (string device in Microphone.devices)
            {
                m_MicrophoneName = device;
                break;
            }
            //m_AudioSource.clip = Microphone.Start(m_MicrophoneName, true, 10, 44100);
            m_AudioSource.clip = Microphone.Start(m_MicrophoneName, true, 120, AudioSettings.outputSampleRate);
            m_AudioSource.loop = true;
            //while (!(Microphone.GetPosition(null) > 0)) { }
            m_AudioSource.Play();
            _ = SampleAcapella(destroyCancellationToken);
        }
        #endregion

        #region API
        public AcapellaTimeseriesPoint CurrentFrame => m_AcapellaTimeseriesPoint;
        #endregion

        #region Acapella Sampler
        private async Awaitable SampleAcapella(CancellationToken destroyToken)
        {
            while (!destroyToken.IsCancellationRequested)
            {
                // Estimate Pitch
                float estimate = m_PitchEstimator.Estimate(m_AudioSource, m_PitchConfiguration);
                if (float.IsNaN(estimate))
                {
                    m_AcapellaTimeseriesPoint.IsSilence = true;
                }
                else
                {
                    m_AcapellaTimeseriesPoint.IsSilence = false;
                    m_AcapellaTimeseriesPoint.Time = m_AudioSource.time;
                    m_AcapellaTimeseriesPoint.Frequency = estimate;
                    m_AcapellaTimeseriesPoint.Note = PitchEstimator.MidiNoteFromFrequency(estimate);
                    //Debug.Log(PitchEstimator.MidiNoteToName(m_AcapellaTimeseriesPoint.Note));
                }

                // Every Other Frame
                await Awaitable.NextFrameAsync();
                await Awaitable.NextFrameAsync();
            }
        }
        #endregion

        public string GetMicrophoneName()
        {
            return m_MicrophoneName;
        }
    }
}
