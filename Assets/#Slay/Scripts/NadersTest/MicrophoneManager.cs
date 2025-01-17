using Slay;
using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;

public class MicrophoneManager : Singer
{
    private AudioClip _microphoneClip;
    private string _microphoneName;
    private int currentNote;
    public StringReference MicrophoneName;

    private void Start()
    {
        ServiceLocator<MicrophoneManager>.Service = this;
    }

    public override void StartSinger()
    {
        m_PitchEstimator = new();
        MicrophoneToAudioClip(MicrophoneName.Value);
    }

    private void MicrophoneToAudioClip(string microphoneName)
    {
        _microphoneClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);

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
        var pointReturn = base.EstimatePitch();

        currentNote = pointReturn.Note;

        return pointReturn;
    }

    public int GetCurrentNote()
    {
        return currentNote;
    }

    private void OnDisable()
    {
        ServiceLocator<MicrophoneManager>.Reset();
    }
}
