using Slay;
using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;

public class PitchDetection : MonoBehaviour
{

    private void Update()
    {
        if(ServiceLocator<MicrophoneManager>.HasService)
        {
            ServiceLocator<MicrophoneManager>.Service.EstimatePitch();
        }
    }
}
