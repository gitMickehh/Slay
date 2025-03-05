using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSinger : Singer
{
    private AudioClip acapellaClip;

    //public float errorFrequency = 0.0f;
    public FloatReference errorFrequency;
    public float pitchModRange = 0.15f;
    public float errorLength = 0.5f;

    private float time;
    private float timeBetweenErrors;
    private bool canMakeErrors;
    private bool erroring;

    [Space]
    public FloatReference currentTrackPosition;

    public void SetUpNPCSinger(AudioClip acapella)
    {
        time = 0;
        //canMakeErrors = (errorFrequency != 0.0f);
        canMakeErrors = (errorFrequency.Value != 0.0f);
        SetRandomErrorTime();

        acapellaClip = acapella;
        m_AudioSource.clip = acapellaClip;
    }

    public void StartNPCSinger()
    {
        m_AudioSource.Play();
    }

    private void Update()
    {
        if (canMakeErrors)
            SingingErrorsUpdate();
    }

    private void SingingErrorsUpdate()
    {
        time += Time.deltaTime;
        if(!erroring)
        {
            if (time >= timeBetweenErrors)
            {
                AddPitch(Random.Range(-pitchModRange, pitchModRange));
                time = 0;
                erroring = true;
            }
        }
        else
        {
            if(time >= errorLength)
            {
                ResetPitch();
                time = 0;
                erroring = false;
            }
        }
    }

    private void SetRandomErrorTime()
    {
        //timeBetweenErrors = Random.Range(errorLength, errorLength + errorFrequency);
        timeBetweenErrors = Random.Range(errorLength, errorLength + errorFrequency.Value);
    }

    private void AddPitch(float pitchModifier)
    {
        m_AudioSource.pitch = 1 + pitchModifier;
    }

    private void ResetPitch()
    {
        m_AudioSource.pitch = 1;
        m_AudioSource.Stop();
        //m_AudioSource.PlayDelayed(currentTrackPosition.Value);
        m_AudioSource.time = currentTrackPosition.Value;
        m_AudioSource.Play();
    }

    public override float GetLoudness()
    {
        var loudness = GetLoudnessFromAudioClip(Mathf.RoundToInt(m_AudioSource.time), acapellaClip);
        return loudness;
    }
}
