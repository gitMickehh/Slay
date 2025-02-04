using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

[SerializeField]
public class MidiNoteTimeStamped
{
    public Note note;
    public double startingTime = 0;
    public double noteLength = 0;

    public MidiNoteTimeStamped(Note givenNote, double noteTime, double metricLength)
    {
        note = givenNote;
        startingTime = noteTime;
        noteLength = metricLength;
    }

    public bool IsEqual(MidiNoteTimeStamped midiNoteInQuestion)
    {
        return midiNoteInQuestion.startingTime == startingTime &&
            midiNoteInQuestion.note.Time == note.Time;
    }

    public double EndTime => startingTime + noteLength;

}
