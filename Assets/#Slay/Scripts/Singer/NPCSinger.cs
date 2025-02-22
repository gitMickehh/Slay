using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSinger : Singer
{
    private AudioClip acapellaClip;

    public void SetUpNPCSinger(AudioClip acapella)
    {
        acapellaClip = acapella;
        m_AudioSource.clip = acapellaClip;
    }

    public void StartNPCSinger()
    {
        m_AudioSource.Play();
    }

    public override float GetLoudness()
    {
        var loudness = GetLoudnessFromAudioClip(Mathf.RoundToInt(m_AudioSource.time),acapellaClip);
        return loudness;
    }
}
