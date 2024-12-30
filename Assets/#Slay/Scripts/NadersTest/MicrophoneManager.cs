using Slay;
using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;

public class MicrophoneManager : Singer
{
    private AudioClip _microphoneClip;
    private string _microphoneName;

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

    public override AcapellaTimeseriesPoint EstimatePitch()
    {
        if (GetLoudnessFromMicrophone() == 0) return new() { IsSilence = true };
        return base.EstimatePitch();
    }

}
