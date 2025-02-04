using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName="Slay/Song", fileName="New Song")]
public class Song : ScriptableObject
{
    [Tooltip("better be a unique name")]public string codename;
    [Header("Music & Instrumental")]
    public AudioClip instrumental;
    public AudioClip acapella;
    [Space]
    public bool useMidi;
    public string midiFileName;
    public AudioClip midiPlayback;


    [Header("Lyrics")]
    public TextAsset lyricsFile;

    public string SongPath => Application.streamingAssetsPath + "/" + codename +".json";
}
