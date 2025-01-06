using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;

public class LyricsManager : MonoBehaviour
{
    public SongReference song;
    Lyrics lyrics;

    private void Start()
    {
        GetLyricsFromJSON(song.Value.lyricsFile.text);
        ServiceLocator<LyricsManager>.Service = this;
    }

    private void GetLyricsFromJSON(string jsonFile)
    {
        lyrics = JsonUtility.FromJson<Lyrics>(jsonFile);
        //Debug.Log(lyrics.lines.Length);
    }

    public string GetLineFromTime(float timeStamp)
    {
        var newLineObject = lyrics.GetLine(timeStamp);
        if (newLineObject != null)
        {
            string line = newLineObject.content;
            return line;
        }

        return "";
    }

    public LyricLine GetLyricLineFromTime(float timeStamp)
    {
        var newLineObject = lyrics.GetLine(timeStamp);
        if (newLineObject != null)
        {
            return newLineObject;
        }

        return null;
    }

    public LyricLine GetLyricLine(int index)
    {
        return lyrics.lines[index];
    }

}
