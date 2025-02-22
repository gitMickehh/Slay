using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;

public class SingerManager : MonoBehaviour
{
    public Singer microphoneSinger;
    public Singer npcSinger;

    public BoolReference singerIsPlayer;
    private Singer currentSinger;

    public Singer CurrentSinger => currentSinger;

    private void Start()
    {
        if (singerIsPlayer.Value)
        {
            currentSinger = Instantiate(microphoneSinger, transform);
        }
        else
        {
            currentSinger = Instantiate(npcSinger, transform);
        }

        ServiceLocator<SingerManager>.Service = this;
        currentSinger.StartSinger();
    }

}
