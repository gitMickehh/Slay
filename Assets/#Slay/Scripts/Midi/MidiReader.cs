using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using Slay;
using Techno;
using System.Linq;

public class MidiReader : MonoBehaviour
{
    [Header("Music")]
    public SongReference songReference;
    private MidiFile midiFile;
    public AudioSource instrumental_source;
    public bool playMidiPlayback;

    //[Header("Prefabs")]
    //public MidiNoteObject note_object_prefab;

    [Header("GameObjects")]
    public MarkerController marker_controller;
    public GameObjectPool notesPool;

    [Header("Generation Settings")]
    public float noteTime = 1f;
    public float unitsPerSecond = 1f;

    [Header("Micorphone Settings")]
    public MicrophoneTrackIndicator microphone_track_indicator;

    Note[] notes_array;
    List<MidiNoteTimeStamped> timestamped_notes = new List<MidiNoteTimeStamped>();
    int maxMidiNote;
    int minMidiNote;
    int midiNoteCount;
    private MidiNoteTimeStamped most_recently_generated_note;
    private List<MidiNoteObject> midiNotesGenerated = new List<MidiNoteObject>();

    [SerializeField]
    private MidiNoteObject currentMiddleNoteMIDINoteObject;

    private void OnEnable()
    {
        timestamped_notes = new List<MidiNoteTimeStamped>();
        midiNotesGenerated = new List<MidiNoteObject>();
        currentMiddleNoteMIDINoteObject = null;

        ReadMidiFile();
        GetDataFromMidi();
    }

    private void Start()
    {
        ProcessNoteData();
        StartSong();
    }

    private void ReadMidiFile()
    {
        //midiFile = MidiFile.Read(Application.streamingAssetsPath + "/MidiFiles/" + songReference.Value.midiFileName.name + ".mid");
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/MidiFiles/" + songReference.Value.midiFileName + ".mid");
    }

    private void GetDataFromMidi()
    {
        ICollection<Note> notes = midiFile.GetNotes();
        notes_array = new Note[notes.Count];
        notes.CopyTo(notes_array, 0);
    }

    private void ProcessNoteData()
    {
        maxMidiNote = int.MinValue;
        minMidiNote = int.MaxValue;
        foreach (var note in notes_array)
        {
            //Debug.Log(NoteToString(note));
            maxMidiNote = Mathf.Max(note.NoteNumber, maxMidiNote);
            minMidiNote = Mathf.Min(note.NoteNumber, minMidiNote);

            timestamped_notes.Add(new MidiNoteTimeStamped(note, GetNoteMetricTime(note), GetNoteLengthMetricTime(note)));
        }

        midiNoteCount = maxMidiNote - minMidiNote;
        //Debug.Log( "max: " + maxMidiNote + ", min: " + minMidiNote + ", notes used: " + midiNoteCount);
    }

    private void StartSong()
    {
        instrumental_source.clip = songReference.Value.instrumental;

        if (playMidiPlayback)
        {
            var instrumentalPlaybackSource = gameObject.AddComponent<AudioSource>();
            instrumentalPlaybackSource.loop = false;
            instrumentalPlaybackSource.playOnAwake = false;
            instrumentalPlaybackSource.Stop();

            instrumentalPlaybackSource.clip = songReference.Value.midiPlayback;
            instrumentalPlaybackSource.Play();
        }

        instrumental_source.loop = false;
        instrumental_source.Play();

        microphone_track_indicator.SetUpTrackIndicator(minMidiNote, midiNoteCount);
    }

    private void Update()
    {
        double time = GetAudioSourceTime();
        float readaheadSizeSeconds = MidpointToStart * unitsPerSecond;
        double readaheadTime = time + (double)readaheadSizeSeconds;

        GenerateNote(readaheadTime);
        UpdateNotes(time);
        UpdateLyrics();

        if(ServiceLocator<MicrophoneManager>.HasService)
            PitchLogic();
    }

    private void PitchLogic()
    {
        AcapellaTimeseriesPoint microphonePoint = ServiceLocator<MicrophoneManager>.Service.EstimatePitch();

        if (currentMiddleNoteMIDINoteObject != null)
            CompareCurrentNote(microphonePoint);
    }

    private void CompareCurrentNote(AcapellaTimeseriesPoint microphonePoint)
    {
        if (microphonePoint.IsSilence) return;

        var modResult = microphonePoint.Note % 12;

        if (modResult == currentMiddleNoteMIDINoteObject.midiNote.note.NoteNumber)
        {
            //exact hit
            currentMiddleNoteMIDINoteObject.Hit(true);
        }
        else if (modResult >= currentMiddleNoteMIDINoteObject.midiNote.note.NoteNumber - 2 || 
            modResult <= currentMiddleNoteMIDINoteObject.midiNote.note.NoteNumber + 2)
        {
            //almost hit
            currentMiddleNoteMIDINoteObject.Hit(false);
        }
        else
        {
            //miss
            currentMiddleNoteMIDINoteObject.Miss();
        }
    }

    private void GenerateNote(double readaheadTime)
    {
        var mNote = GetMidiNodeInCurrentTime(readaheadTime);
        if (mNote != null)
        {
            if (most_recently_generated_note != null)
            {
                if (most_recently_generated_note.IsEqual(mNote))
                    return;
            }

            most_recently_generated_note = mNote;
            //Debug.Log(NoteToString(mNote.note));
            GenerateNote(most_recently_generated_note);
        }
    }

    private void UpdateNotes(double audioTime)
    {
        foreach (var noteObject in midiNotesGenerated)
        {
            //noteObject.UpdateNote(audioTime, unitsPerSecond, marker_controller.TrackStartX, marker_controller.TrackEndX);
            noteObject.UpdateNote(audioTime, noteTime, marker_controller.TrackStartX, marker_controller.TrackEndX);
        }

        CheckForMiddleNote();
    }

    private void CheckForMiddleNote()
    {
        var noteObject = midiNotesGenerated.Find(x=>x.isAtMidPoint);
        if(noteObject != null)
        {
            if (currentMiddleNoteMIDINoteObject == null ||
                !currentMiddleNoteMIDINoteObject.midiNote.IsEqual(noteObject.midiNote))
            {
                currentMiddleNoteMIDINoteObject = noteObject;
            }
        }
        else
        {
            currentMiddleNoteMIDINoteObject = null;
        }
    }

    private void GenerateNote(MidiNoteTimeStamped newNote)
    {
        MidiNoteObject noteObject = notesPool.Get().GetComponent<MidiNoteObject>();
        noteObject.SetupNote(GetNoteStartingPoint(newNote.note), GetAudioSourceTime(), newNote);
        midiNotesGenerated.Add(noteObject);
    }

    private double GetAudioSourceTime()
    {
        return (double)instrumental_source.timeSamples / instrumental_source.clip.frequency;
    }

    private MidiNoteTimeStamped GetMidiNodeInCurrentTime(double givenTime)
    {
        foreach (var item in timestamped_notes)
        {
            if (item.EndTime < givenTime)
                continue;

            if (item.startingTime < givenTime)
                return item;
            else
                return null;
        }

        return null;
    }

    private Vector3 GetNoteStartingPoint(Note note)
    {
        float positionX = marker_controller.TrackStartX;
        float positionY =
        marker_controller.TrackStartY
            + (marker_controller.TrackHeight * ((note.NoteNumber - minMidiNote) / (float)midiNoteCount));
        float positionZ = marker_controller.TrackZ;
        return new Vector3(positionX, positionY, positionZ);
    }

    private string NoteToString(Note note)
    {
        return note.NoteName + note.Octave + ","
                + note.NoteNumber + ", game note: "
                + PitchEstimator.MidiNoteToName(note.NoteNumber) + ": "
                + note.Length + ", "
                + note.Time + ", metric time: "
                + GetNoteMetricTime(note) + ", metric length: "
                + GetNoteLengthMetricTime(note);
    }

    private double GetNoteMetricTime(Note note)
    {
        var metricTimestamp = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, midiFile.GetTempoMap());
        return (
            (double)(metricTimestamp.Minutes * 60f) +
            (double)metricTimestamp.Seconds +
            (double)(metricTimestamp.Milliseconds / 1000f)
            );
    }

    private double GetNoteLengthMetricTime(Note note)
    {
        var metricTimestamp = TimeConverter.ConvertTo<MetricTimeSpan>(note.Length, midiFile.GetTempoMap());
        return (
            (double)(metricTimestamp.Minutes * 60f) +
            (double)metricTimestamp.Seconds +
            (double)(metricTimestamp.Milliseconds / 1000f)
            );
    }

    private void UpdateLyrics()
    {
        if (ServiceLocator<LyricsUIManager>.HasService)
            ServiceLocator<LyricsUIManager>.Service.UpdateSubtitles(instrumental_source.time);
    }

    private float MidpointToStart => Mathf.Abs(marker_controller.TrackStartX - marker_controller.TrackMidpoint.x);
}
