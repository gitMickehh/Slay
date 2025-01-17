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
            singer = Instantiate(microphoneSinger, transform);
        }
        else
        {
            singer = Instantiate(npcSinger, transform);
        }

        ServiceLocator<SingerManager>.Service = this;
        singer.StartSinger();
    }

}
