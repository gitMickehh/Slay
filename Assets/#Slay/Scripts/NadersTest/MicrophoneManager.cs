using Slay;
using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;

public class MicrophoneManager : MonoBehaviour
{
    public int sampleWindow = 64;

    private AudioClip _microphoneClip;
    private string _microphoneName;

    [Header("Loudness Settings")]
    [SerializeField]
    private float loudnessSensibility = 5f;
    [SerializeField]
    private float threshold = 0.1f;

    [Header("Hearing Back")]
    [SerializeField]
    private AudioSource m_AudioSource;

    [Header("Pitch Settings")]
    [SerializeField]
    private PitchEstimatorConfiguration m_PitchConfiguration;
    private PitchEstimator m_PitchEstimator;

    private void Start()
    {
        m_PitchEstimator = new();
        MicrophoneToAudioClip(0);

        ServiceLocator<MicrophoneManager>.Service = this;
    }

    private void MicrophoneToAudioClip(int microphoneIndex)
    {
        _microphoneName = Microphone.devices[microphoneIndex];
        _microphoneClip = Microphone.Start(_microphoneName, true, 20, AudioSettings.outputSampleRate);

        m_AudioSource.clip = _microphoneClip;
        m_AudioSource.loop = true;
        m_AudioSource.Play();
    }

    public float GetLoudnessFromMicrophone()
    {
        var loudness = GetLoudnessFromAudioClip(Microphone.GetPosition(_microphoneName), _microphoneClip) * loudnessSensibility;
        if (loudness < threshold) loudness = 0;

        return loudness;
    }

    private float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
    {
        int startPosition = clipPosition - sampleWindow;
        if (startPosition <= 0) return 0;

        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);

        float totalLoudness = 0;

        foreach (var sample in waveData)
        {
            totalLoudness += Mathf.Abs(sample);
        }

        return totalLoudness / sampleWindow;        //mean loudness
    }

    public AcapellaTimeseriesPoint EstimatePitch()
    {
        AcapellaTimeseriesPoint m_AcapellaTimeseriesPoint = new() { IsSilence = true };
        if (GetLoudnessFromMicrophone() == 0) return m_AcapellaTimeseriesPoint;

        float estimate = m_PitchEstimator.Estimate(m_AudioSource, m_PitchConfiguration);

        if (!float.IsNaN(estimate))
        {
            m_AcapellaTimeseriesPoint.IsSilence = false;
            m_AcapellaTimeseriesPoint.Time = m_AudioSource.time;
            m_AcapellaTimeseriesPoint.Frequency = estimate;
            m_AcapellaTimeseriesPoint.Note = PitchEstimator.MidiNoteFromFrequency(estimate);
        }

#if UNITY_EDITOR
        //if (!m_AcapellaTimeseriesPoint.IsSilence) Debug.Log(PitchEstimator.MidiNoteToName(m_AcapellaTimeseriesPoint.Note));
#endif

        return m_AcapellaTimeseriesPoint;
    }

}
