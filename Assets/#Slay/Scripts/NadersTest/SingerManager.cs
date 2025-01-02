using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;

public class SingerManager : MonoBehaviour
{
    public Singer microphoneSinger;
    public Singer npcSinger;


    public BoolReference singerIsPlayer;
    private Singer singer;

    private void Start()
    {
        if (singerIsPlayer.Value)
        {
            singer = Instantiate(microphoneSinger);
        }
        else
        {
            singer = Instantiate(npcSinger);
        }

        ServiceLocator<SingerManager>.Service = this;
    }

}
