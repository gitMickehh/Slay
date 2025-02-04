using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public void SetIndexesOfLines()
    {
        for (var i = 0; i < lines.Length; i++) { 
            lines[i].index = i;
        }
    }

    //public int GetLineIndex(string line)
    //{
    //    int index = -1;
    //    index = lines.ToList().FindIndex(x=>x.content == line);
    //    return index;
    //}
}
