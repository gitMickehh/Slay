using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;

public class ScaleFromMicrophone : MonoBehaviour
{
    public Vector3 minScale, maxScale;
    //public AudioLoudnessDetector detector;
    //public float loudnessSensibility = 2f;
    //public float threshold = 0.05f;

    private void Update()
    {
        if (ServiceLocator<MicrophoneSinger>.HasService)
        {
            //var loudness = ServiceLocator<MicrophoneSinger>.Service.GetLoudnessFromMicrophone();
            var loudness = ServiceLocator<MicrophoneSinger>.Service.GetLoudness();
            transform.localScale = Vector3.Lerp(minScale, maxScale, loudness);
        }
    }

}
