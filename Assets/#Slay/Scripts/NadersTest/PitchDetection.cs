using Slay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchDetection : MonoBehaviour
{
    [SerializeField]
    private PitchEstimatorConfiguration m_PitchConfiguration;

    private PitchEstimator m_PitchEstimator;

    private AudioClip _microphoneClip;
    private string _microphoneName;

    [SerializeField]
    private AudioSource m_AudioSource;

    private void Start()
    {
        m_PitchEstimator = new();
        
        _microphoneName = Microphone.devices[0];
        _microphoneClip = Microphone.Start(_microphoneName, true, 20, AudioSettings.outputSampleRate);
        
        m_AudioSource.clip = _microphoneClip;
        m_AudioSource.Play();
        m_AudioSource.loop = true;
    }

    private void Update()
    {
        float estimate = m_PitchEstimator.Estimate(m_AudioSource, m_PitchConfiguration);

        AcapellaTimeseriesPoint m_AcapellaTimeseriesPoint = new() { IsSilence = true };

        //Debug.Log(estimate + " | audio source playing: " + m_AudioSource.isPlaying);

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
        }

        if(!m_AcapellaTimeseriesPoint.IsSilence) Debug.Log(PitchEstimator.MidiNoteToName(m_AcapellaTimeseriesPoint.Note));
    }
}
