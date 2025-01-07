using Slay;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Techno;
using UnityEngine;
using UnityEngine.UIElements;

public class MicrophoneTrackIndicator : MonoBehaviour
{

    [SerializeField]
    private MarkerController m_MarkerController;

    [SerializeField]
    private Rigidbody m_Rigidbody;

    private bool ready;
    private int m_minMidiNote;
    private int m_midiNoteCount;

    private void Start()
    {
        ready = false;
    }

    public void SetUpTrackIndicator(int minMidiNote, int midiNoteCount)
    {
        m_minMidiNote = minMidiNote;
        m_midiNoteCount = midiNoteCount;

        ready = true;
    }

    private void Update()
    {
        if (ready)
            UpdateTrackIndicator(m_minMidiNote, m_midiNoteCount);
    }

    private void UpdateTrackIndicator(int minMidiNote, int midiNoteCount)
    {
        var note = ServiceLocator<MicrophoneManager>.Service.GetCurrentNote();

        if (note == 0)
        {
            m_Rigidbody.position = new Vector3(TrackXMidpoint, TrackYMidpoint, TrackZ);
        }
        else
        {
            m_Rigidbody.position = GetTrackIndicatorPosition(
                note,
                minMidiNote,
                midiNoteCount);
        }

    }

    private Vector3 GetTrackIndicatorPosition(int microphoneNote, int minMidiNote, int midiNoteCount)
    {
        float positionY =
        TrackStartY
            + (TrackHeight * ((microphoneNote - minMidiNote) / (float)midiNoteCount));

        return new Vector3(TrackXMidpoint, positionY, TrackZ);
    }

    #region Helpers
    private float TrackHeight => m_MarkerController.TrackHeight;
    private float TrackXMidpoint => ((m_MarkerController.TrackEndX - m_MarkerController.TrackStartX) / 2f + m_MarkerController.TrackStartX);
    private float TrackYMidpoint => ((m_MarkerController.TrackEndY - m_MarkerController.TrackStartY) / 2f + m_MarkerController.TrackStartY);
    private float TrackStartY => m_MarkerController.TrackStartY;
    private float TrackZ => m_MarkerController.TrackZ;
    #endregion
}
