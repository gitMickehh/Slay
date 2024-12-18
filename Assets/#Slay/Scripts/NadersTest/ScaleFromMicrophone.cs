using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleFromMicrophone : MonoBehaviour
{
    public Vector3 minScale, maxScale;
    public AudioLoudnessDetector detector;
    public float loudnessSensibility = 2f;
    public float threshold = 0.05f;

    private void Update()
    {
        float loudness = detector.GetLoudnessFromMicrophone() * loudnessSensibility;
        //Debug.Log(loudness);
        if (loudness < threshold) loudness = 0;
        //else loudness *= loudnessSensibility;

        transform.localScale = Vector3.Lerp(minScale, maxScale, loudness);
    }

}
