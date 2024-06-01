using System.Collections.Generic;
using System.IO;
using Techno;
using UnityEngine;

namespace Slay
{
    public class TrackController : MonoBehaviour
    {
        #region Inspector
        [SerializeField]
        private AudioSource m_AudioSourceInstrumental;

        [SerializeField]
        private AudioSource m_AudioSourceAcapella;

        [SerializeField]
        private GameObject m_NotePrefab;

        [SerializeField]
        private Transform m_Midpoint;

        [SerializeField]
        private MarkerController m_MarkerController;

        [SerializeField]
        private Transform m_NotesContainer;

        [SerializeField]
        private Transform m_NoteGraveyard;

        // We'll just say 1 unit in space == 1 second in time by default
        [SerializeField]
        private float m_UnitsPerSecond = 1f;

        [SerializeField]
        private float m_SamplesPerSecond = 2f;

        [SerializeField]
        private float m_ProjectileForce = 20f;

        [SerializeField]
        private float m_ProjectileYBeforeRecycle = -20f;

        [SerializeField]
        private bool m_PlayAcapella = false;
        #endregion

        #region State
        private AcapellaTimeseries m_Timeseries;
        private float m_TimeSinceLastNoteSample;
        private List<NoteController> m_NoteConveyor;
        private List<NoteController> m_NoteBlasted;
        private int m_MaxMidiNote;
        private int m_MinMidiNote;
        private int m_MidiNoteCount;
        #endregion

        #region Unity Lifecycle Events
        private void Awake()
        {
            if (File.Exists(AcapellaController.SongPath))
            {
                m_Timeseries = JsonUtility.FromJson<AcapellaTimeseries>(
                    File.ReadAllText(AcapellaController.SongPath)
                );
                m_MaxMidiNote = int.MinValue;
                m_MinMidiNote = int.MaxValue;
                for (int i = 0; i < m_Timeseries.Points.Count; i++)
                {
                    AcapellaTimeseriesPoint point = m_Timeseries.Points[i];
                    if (point.IsSilence)
                        continue;
                    m_MaxMidiNote = Mathf.Max(point.Note, m_MaxMidiNote);
                    m_MinMidiNote = Mathf.Min(point.Note, m_MinMidiNote);
                }
                m_MidiNoteCount = m_MaxMidiNote - m_MinMidiNote;
            }
            else
                gameObject.SetActive(false);
        }

        private void Start()
        {
            m_NoteBlasted = new();
            m_NoteConveyor = new();
            m_AudioSourceInstrumental.Play();
            if (m_PlayAcapella)
                m_AudioSourceAcapella.Play();
            m_TimeSinceLastNoteSample = TimeBetweenSamples;
        }

        private void Update()
        {
            // Sample
            float time = m_AudioSourceInstrumental.time;
            float readaheadSizeSeconds = MidpointToStart * m_UnitsPerSecond;
            m_TimeSinceLastNoteSample += Time.deltaTime;
            if (m_TimeSinceLastNoteSample > TimeBetweenSamples)
            {
                // Reset time since last sample
                m_TimeSinceLastNoteSample = 0f;

                // Fetch sample at readahead time
                float readaheadTime = time + readaheadSizeSeconds;
                AcapellaTimeseriesPoint point = m_Timeseries.GetPointAtTime(readaheadTime);

                // Create note if sample is not silent
                if (!point.IsSilence)
                {
                    NoteController newNote;
                    if (m_NoteGraveyard.childCount == 0)
                        newNote = Instantiate(m_NotePrefab).GetComponent<NoteController>();
                    else
                        newNote = m_NoteGraveyard.GetChild(0).GetComponent<NoteController>();

                    newNote.Point = point;
                    float positionX = TrackStartX;
                    float positionY =
                        TrackStartY
                        + (TrackHeight * ((point.Note - m_MinMidiNote) / (float)m_MidiNoteCount));
                    float positionZ = TrackZ;
                    newNote.transform.position = new Vector3(positionX, positionY, positionZ);
                    newNote.Rigidbody.position = new Vector3(positionX, positionY, positionZ);
                    newNote.transform.SetParent(m_NotesContainer);
                    m_NoteConveyor.Add(newNote);
                }
            }

            // Move
            AcapellaTimeseriesPoint microphonePoint = ServiceLocator<MicrophoneController>
                .Service
                .CurrentFrame;
            float trackEndX = TrackEndX;
            for (int i = m_NoteConveyor.Count - 1; i >= 0; i--)
            {
                NoteController note = m_NoteConveyor[i];
                float startTime = note.Point.Time - readaheadSizeSeconds;
                if (startTime > time)
                    continue;
                float normalizedElapsed = (time - startTime) / (2 * readaheadSizeSeconds);
                float expectedPositionX = Mathf.Lerp(TrackStartX, TrackEndX, normalizedElapsed);

                Vector3 newPosition = note.Rigidbody.position;
                newPosition.x = expectedPositionX;
                note.Rigidbody.position = newPosition;

                // Do we blast?
                // Debug.Log($"Britney {note.Point.Note} Eric {microphonePoint.Note}");
                if (
                    note.IsAtMidline
                    && !microphonePoint.IsSilence
                    && microphonePoint.Note == note.Point.Note
                )
                {
                    m_NoteConveyor.RemoveAt(i);
                    m_NoteBlasted.Add(note);
                    note.Rigidbody.isKinematic = false;
                    note.Rigidbody.useGravity = true;
                    note.Rigidbody.AddForce(Vector3.back * m_ProjectileForce, ForceMode.Impulse);
                }
                // Remove note
                else if (newPosition.x >= trackEndX)
                {
                    m_NoteConveyor.RemoveAt(i);
                    GraveyardNote(note);
                }
            }

            // Clean up projectiles
            for (int i = m_NoteBlasted.Count - 1; i >= 0; i--)
            {
                NoteController note = m_NoteBlasted[i];
                if (note.transform.position.y < m_ProjectileYBeforeRecycle)
                {
                    m_NoteBlasted.RemoveAt(i);
                    GraveyardNote(note);
                }
            }
        }
        #endregion

        #region Helpers
        private void GraveyardNote(NoteController note)
        {
            note.Rigidbody.useGravity = false;
            if (!note.Rigidbody.isKinematic)
            {
                note.Rigidbody.velocity = Vector3.zero;
                note.Rigidbody.angularVelocity = Vector3.zero;
                note.Rigidbody.isKinematic = true;
            }
            note.transform.SetParent(m_NoteGraveyard);
            note.transform.localPosition = Vector3.zero;
            note.transform.localRotation = Quaternion.identity;
            note.Rigidbody.position = note.transform.position;
            note.Rigidbody.rotation = note.transform.rotation;
        }

        private float MidpointToStart => Mathf.Abs(TrackStartX - m_Midpoint.transform.position.x);
        private float TimeBetweenSamples => 1f / m_SamplesPerSecond;
        private float TrackHeight => m_MarkerController.TrackHeight;
        private float TrackStartX => m_MarkerController.TrackStartX;
        private float TrackStartY => m_MarkerController.TrackStartY;
        private float TrackEndX => m_MarkerController.TrackEndX;
        private float TrackZ => m_MarkerController.TrackZ;
        #endregion
    }
}
