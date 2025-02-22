using Slay;
using Techno;
using UnityEngine;

public class MicrophoneTrackIndicator : MonoBehaviour
{

    [SerializeField]
    private MarkerController m_MarkerController;

    [SerializeField]
    //private Rigidbody m_Rigidbody;

    private bool ready;
    private int m_minMidiNote;
    private int m_midiNoteCount;

    [Header("Materials")]
    public MeshRenderer indicator_mesh;
    public Material ActiveVocalInputMaterial;
    public Material ActiveNoInputMaterial;
    public Material InactiveVocalInputMaterial;
    public Material InactiveNoInputMaterial;
    bool incomingInput;         //is true when there is input from the microphone
    private bool activeState;   //is true when there is a note to sing in the MidPoint. Otherwise it is false

    [Header("Lerping Speed")]
    public float lerpSpeed = 0.5f;
    Vector3 currentTargetPosition;
    float lerpTime;

    public void SetUpTrackIndicator(int minMidiNote, int midiNoteCount)
    {
        m_minMidiNote = minMidiNote;
        m_midiNoteCount = midiNoteCount;
        
        incomingInput = false;
        indicator_mesh.material = InactiveNoInputMaterial;

        currentTargetPosition = Vector3.zero;
        lerpTime = 0;

        ready = true;
    }

    private void Update()
    {
        if (ready)
            UpdateTrackIndicator(m_minMidiNote, m_midiNoteCount);
    }

    private void UpdateTrackIndicator(int minMidiNote, int midiNoteCount)
    {
        if (!ServiceLocator<SingerManager>.HasService) return;
        if (ServiceLocator<SingerManager>.Service.CurrentSinger == null) return;

        //var note = ServiceLocator<MicrophoneSinger>.Service.GetCurrentNote();
        var note = ServiceLocator<SingerManager>.Service.CurrentSinger.GetCurrentNote();

        if (note == 0)
        {
            //Debug.Log("note = 0");

            if(incomingInput)
            {
                incomingInput = false;
                indicator_mesh.material = activeState? ActiveNoInputMaterial : InactiveNoInputMaterial;
                transform.localPosition = new Vector3(TrackXMidpoint, TrackYMidpoint, TrackZ);
            }
        }
        else
        {
            //Debug.Log("note != 0");
            if (!incomingInput)
            {
                incomingInput = true;
                indicator_mesh.material = activeState? ActiveVocalInputMaterial : InactiveVocalInputMaterial;

                lerpTime = 0;
                //currentTargetPosition = Vector3.zero;
                currentTargetPosition = transform.localPosition;
            }

            //transform.localPosition = GetTrackIndicatorPosition(
            //    note,
            //    minMidiNote,
            //    midiNoteCount);

            var newTargetPosition = GetTrackIndicatorPosition(note, minMidiNote, midiNoteCount);
            if(currentTargetPosition != newTargetPosition)
            {
                lerpTime = 0;
                currentTargetPosition = newTargetPosition;
            }

            lerpTime += Time.deltaTime;
            lerpTime = Mathf.Clamp01(lerpTime / lerpSpeed);

            transform.localPosition = Vector3.Lerp(transform.localPosition, 
                GetTrackIndicatorPosition(note,minMidiNote,midiNoteCount), lerpTime); 
        }

    }

    private Vector3 GetTrackIndicatorPosition(int microphoneNote, int minMidiNote, int midiNoteCount)
    {
        float positionY =
        m_MarkerController.TrackStartY
            + (m_MarkerController.TrackHeight * ((microphoneNote - minMidiNote) / (float)midiNoteCount));

        return new Vector3(TrackXMidpoint, positionY, TrackZ);
    }

    public void SetActive(bool newActiveState)
    {
        activeState = newActiveState;


    }


    #region Helpers
    private float TrackXMidpoint => ((m_MarkerController.TrackEndX - m_MarkerController.TrackStartX) / 2f + m_MarkerController.TrackStartX);
    private float TrackYMidpoint => ((m_MarkerController.TrackEndY - m_MarkerController.TrackStartY) / 2f + m_MarkerController.TrackStartY);
    private float TrackZ => m_MarkerController.TrackZ;
    #endregion
}
