using Slay;
using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;

public class MicrophoneSinger : Singer
{
    private AudioClip _microphoneClip;
    private string _microphoneName;
    //private int currentNote;
    public StringReference MicrophoneName;

    [Header("Loudness Settings")]
    public float loudnessSensibility = 5f;
    public float threshold = 0.1f;

    private string currentMicrophoneName;

    private void Start()
    {
        ServiceLocator<MicrophoneSinger>.Service = this;
        currentMicrophoneName = "";
    }

    public override void StartSinger()
    {
        m_PitchEstimator = new();
        MicrophoneToAudioClip(MicrophoneName.Value);
    }

    public void RestartMicrophoneSinger()
    {
        if (!string.IsNullOrEmpty(currentMicrophoneName))
        {
            if (currentMicrophoneName == MicrophoneName.Value)
                return;

            m_AudioSource.Stop();
            Microphone.End(currentMicrophoneName);
        }
        else
        {
            StartSinger();
            return;
        }

        MicrophoneToAudioClip(MicrophoneName.Value);
    }

    private void MicrophoneToAudioClip(string microphoneName)
    {
        currentMicrophoneName = microphoneName;

        _microphoneClip = Microphone.Start(microphoneName, true, 1, AudioSettings.outputSampleRate);

        m_AudioSource.clip = _microphoneClip;
        m_AudioSource.loop = true;
        m_AudioSource.Play();
    }

    public override float GetLoudness()
    {
        var loudness = GetLoudnessFromAudioClip(Microphone.GetPosition(_microphoneName), _microphoneClip) * loudnessSensibility;
        if (loudness < threshold) loudness = 0;

        return loudness;
    }

    //public float GetLoudnessFromMicrophone()
    //{
    //    var loudness = GetLoudnessFromAudioClip(Microphone.GetPosition(_microphoneName), _microphoneClip) * loudnessSensibility;
    //    if (loudness < threshold) loudness = 0;

    //    return loudness;
    //}

    public override AcapellaTimeseriesPoint EstimatePitch()
    {
        //if (GetLoudnessFromMicrophone() == 0) return new() { IsSilence = true };
        if (GetLoudness() == 0) return new() { IsSilence = true };
        var pointReturn = base.EstimatePitch();

        return pointReturn;
    }

    private void OnDisable()
    {
        ServiceLocator<MicrophoneSinger>.Reset();
    }
}
