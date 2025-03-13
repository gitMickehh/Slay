using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeBehaviour : MonoBehaviour
{
    Vector3 originalPosition;

    public float shakeDuration = 0.3f;
    public float shakeStrength = 1.0f;
    public int shakeVibrato = 1;

    private void OnEnable()
    {
        originalPosition = transform.position;    
    }

    public void SetNewOriginalPosition()
    {
        originalPosition = transform.position;
    }

    public void Shake()
    {
        transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, 90, false, true, ShakeRandomnessMode.Harmonic).OnComplete(OnCompleteShake);
    }

    private void OnCompleteShake()
    {
        transform.position = originalPosition;
    }
}
