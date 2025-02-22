using Slay;
using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;

public class MidPoint : MonoBehaviour
{
    [SerializeField]
    [Tooltip("don't touch, it's for debugging!")]
    private MidiNoteObject currentOccupant;
    private bool HasOccupant => currentOccupant != null;

    public bool CurrentNote => currentOccupant;

    public MicrophoneTrackIndicator microphoneFeedbackIndicator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "singing_note")
        {
            FillOccupant(other.GetComponentInParent<MidiNoteObject>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "singing_note")
        {
            RemoveOccupant(other.GetComponentInParent<MidiNoteObject>());
        }
    }

    private void FillOccupant(MidiNoteObject newNote)
    {
        if(HasOccupant)
        {
            currentOccupant.UnHighlightNote();
        }

        currentOccupant = newNote;
        currentOccupant.HighlightNote();

        IsFilled();
    }

    private void RemoveOccupant(MidiNoteObject noteExiting)
    {
        if (HasOccupant)
        {
            noteExiting.UnHighlightNote();

            if (currentOccupant.midiNote.IsEqual(noteExiting.midiNote))
            {
                currentOccupant = null;
                IsEmpty();
            }
        }
    }

    //same as RemoveOccupant but it doesn't affect the note's material!
    private void ClearOccupant(MidiNoteObject noteToBeCleared)
    {
        if(HasOccupant)
        {
            if (currentOccupant.midiNote.IsEqual(noteToBeCleared.midiNote))
            {
                currentOccupant = null;
                IsEmpty();
            }
        }
    }

    private void IsFilled()
    {
        microphoneFeedbackIndicator.SetActive(true);
    }

    private void IsEmpty()
    {
        microphoneFeedbackIndicator.SetActive(false);
    }

    private void Update()
    {
        if (ServiceLocator<SingerManager>.HasService)
        {
            if(ServiceLocator<SingerManager>.Service.CurrentSinger != null && HasOccupant)
                PitchLogic();
        }
    }

    private void PitchLogic()
    {
        //AcapellaTimeseriesPoint microphonePoint = ServiceLocator<MicrophoneSinger>.Service.EstimatePitch();
        AcapellaTimeseriesPoint microphonePoint = ServiceLocator<SingerManager>.Service.CurrentSinger.EstimatePitch();
        CompareCurrentNote(microphonePoint);
    }

    private void CompareCurrentNote(AcapellaTimeseriesPoint microphonePoint)
    {
        if (microphonePoint.IsSilence) return;

        //var modResult = microphonePoint.Note % 12;
        var modResult = microphonePoint.Note;

        if (modResult == currentOccupant.midiNote.note.NoteNumber)
        {
            //exact hit
            currentOccupant.Hit(true);
            ClearOccupant(currentOccupant);
        }
        //else if (modResult >= currentOccupant.midiNote.note.NoteNumber - 2 ||
        //    modResult <= currentOccupant.midiNote.note.NoteNumber + 2)
        //{
        //    //almost hit
        //    currentOccupant.Hit(false);
        //    ClearOccupant(currentOccupant);
        //}
        else
        {
            //miss
            currentOccupant.Miss();
        }
    }

}
