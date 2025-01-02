using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
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
}
