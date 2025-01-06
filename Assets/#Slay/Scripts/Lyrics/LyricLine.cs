using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LyricLine
{
    public float begin;
    public float end;
    public string content;

    public LyricLine(float b, float e, string c)
    {
        begin = b;
        end = e;
        content = c;
    }

    public bool IsEqual(LyricLine other)
    {
        if (other == null) return false;
        return begin == other.begin && end == other.end && content == other.content;
    }
}
