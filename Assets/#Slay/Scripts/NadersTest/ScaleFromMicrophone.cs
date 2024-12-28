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
        if (ServiceLocator<MicrophoneManager>.HasService)
        {
            var loudness = ServiceLocator<MicrophoneManager>.Service.GetLoudnessFromMicrophone();
            transform.localScale = Vector3.Lerp(minScale, maxScale, loudness);
        }
    }

}
