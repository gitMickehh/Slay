using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;

public class Test_LyricsAcapella : MonoBehaviour
{
    public AudioSource acapellaSource;
    //private string currentLine;

    private void Start()
    {
        //currentLine = "";
        acapellaSource.Play();
    }

    private void Update()
    {
        //var line = ServiceLocator<LyricsManager>.Service.GetLineFromTime(acapellaSource.time);        
        //if(line != "")
        //{
        //    if(currentLine != line)
        //    {
        //        currentLine = line;
        //        Debug.Log(currentLine);
        //    }
        //}

        if(ServiceLocator<LyricsUIManager>.HasService)
            ServiceLocator<LyricsUIManager>.Service.UpdateSubtitles(acapellaSource.time);
    }
}
