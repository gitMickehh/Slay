using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Slay/Song", fileName="New Song")]
public class Song : ScriptableObject
{
    [Tooltip("better be a unique name")]public string codename;
    public AudioClip instrumental;
    public AudioClip acapella;

    public string SongPath => Application.streamingAssetsPath + "/" + codename +".json";
}
