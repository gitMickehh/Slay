using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lyrics
{
    public LyricLine[] lines;

    public LyricLine GetLine(float timestamp)
    {
        foreach (var line in lines) { 
            if(line.end < timestamp)
                continue;

            if (line.begin < timestamp)
                return line;
            else
                return null;
        }

        return null;
    }
}
