using Slay;
using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;

public class Singer : MonoBehaviour
{
    public int sampleWindow = 64;

    [Header("Loudness Settings")]
    public float loudnessSensibility = 5f;
    public float threshold = 0.1f;

    [Header("Hearing Back")]
    public AudioSource m_AudioSource;

    [Header("Pitch Settings")]
    public PitchEstimatorConfiguration m_PitchConfiguration;
    public PitchEstimator m_PitchEstimator;

    public virtual void StartSinger()
    {
        m_PitchEstimator = new();
    }

    public float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
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

    public virtual AcapellaTimeseriesPoint EstimatePitch()
    {
        AcapellaTimeseriesPoint m_AcapellaTimeseriesPoint = new() { IsSilence = true };

        float estimate = m_PitchEstimator.Estimate(m_AudioSource, m_PitchConfiguration);

        if (!float.IsNaN(estimate))
        {
            m_AcapellaTimeseriesPoint.IsSilence = false;
            m_AcapellaTimeseriesPoint.Time = m_AudioSource.time;
            m_AcapellaTimeseriesPoint.Frequency = estimate;
            m_AcapellaTimeseriesPoint.Note = PitchEstimator.MidiNoteFromFrequency(estimate);
        }

        return m_AcapellaTimeseriesPoint;
    }

}
