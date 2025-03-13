using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Linq;
using Techno;

public class MainMenuUI : MonoBehaviour
{
    public SongReference songReference;
    public StringReference microphoneName;
    //public BoolReference singerIsPlayer;
    //public FloatReference singerErrorFrequency;
    public GameStateScriptableObject gameStateScriptableObject;

    //[Header("Songs")]
    //public Song britneySong;
    //public Song charlieSong;
    //public Song MileySong;
    //public Song PPCSong;

    public List<Song> songs;
    //public Song MIDI_Britney;

    DropdownField microphonesDropDownField;
    ProgressBar microphoneSignalBar;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        DropdownField songDropDownField = root.Q<DropdownField>("SongChoice");
        songDropDownField.choices = GetSongsAsList();
        songDropDownField.RegisterValueChangedCallback(evt => SetSongByString(evt.newValue));
        songDropDownField.index = 0;

        microphonesDropDownField = root.Q<DropdownField>("microphoneDropDown");
        microphonesDropDownField.choices = Microphone.devices.ToList<string>();

        microphoneSignalBar = root.Q<ProgressBar>("audioSignal");
        microphoneSignalBar.value = 0;

        Button startButton = root.Q<Button>("startButton");
        startButton.clicked += StartNextScene;

        Toggle singerToggle = root.Q<Toggle>("AttackModeToggle");
        singerToggle.RegisterValueChangedCallback(evt => SingerToggleValueChanged(evt.newValue));
        //singerToggle.value = singerIsPlayer.Value;
        singerToggle.value = gameStateScriptableObject.attackerIsPlayer;

        Slider singerErrorFrequencySlider = root.Q<Slider>("errFqSlider");
        singerErrorFrequencySlider.RegisterValueChangedCallback(evt => SingerErrorFrequencyChanged(evt.newValue));
        //singerErrorFrequencySlider.value = singerErrorFrequency.Value;
        singerErrorFrequencySlider.value = gameStateScriptableObject.NPCSingerErrorFq;

        Toggle defenderModeToggle = root.Q<Toggle>("DefenderModeToggle");
        defenderModeToggle.RegisterValueChangedCallback(evt => DefenderToggleValueChanged(evt.newValue));
        defenderModeToggle.value = gameStateScriptableObject.defenderIsPlayer;

        Slider defenderPerfectionSlider = root.Q<Slider>("defenderPerfection");
        defenderPerfectionSlider.RegisterValueChangedCallback(evt => DefenderPerfectionValueChanged(evt.newValue));
        defenderPerfectionSlider.value = gameStateScriptableObject.defenderPerfectionLevel;

        //Button startMidiButton = root.Q<Button>("startMidiButton");
        //startMidiButton.clicked += StartMidiButton;

        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        microphonesDropDownField.RegisterValueChangedCallback(evt => SetMicrophoneName(evt.newValue));
        microphonesDropDownField.index = 0;
        //ServiceLocator<MicrophoneManager>.Service.StartSinger();
    }

    private List<string> GetSongsAsList()
    {
        var returnList = new List<string>();

        foreach (Song song in songs)
        {
            returnList.Add(song.codename);
        }

        return returnList;
    }

    private void SetSongByString(string songName)
    {
        var song = songs.Find(song => song.codename == songName);
        if (song != null)
        {
            SetSongReference(song);
        }
        else
        {
            Debug.LogError("Something is wrong, song is not found, codename: " + songName);
        }
    }

    private void SetSongReference(Song song)
    {
        songReference.Value = song;
    }

    private void SetMicrophoneName(string microphoneDeviceName)
    {
        ServiceLocator<MicrophoneSinger>.Service.RestartMicrophoneSinger();
        microphoneName.Value = microphoneDeviceName;
    }

    private void StartNextScene()
    {
        SceneManager.LoadScene(1);
    }

    //private void StartMidiButton()
    //{
    //    songReference.Value = MIDI_Britney;
    //    SceneManager.LoadScene(2);
    //}

    private float GetSignalFloat(float loudness, float maxLoudness)
    {
        var value = loudness / maxLoudness * 100f;
        return value;
    }

    private void FixedUpdate()
    {
        if (microphonesDropDownField.choices.Count == 0)
        { 
            microphonesDropDownField.SetEnabled(false); 
        }
        else
        {
            microphonesDropDownField.SetEnabled(true);
            microphonesDropDownField.choices = Microphone.devices.ToList<string>();

            //microphoneSignalBar.value = GetSignalFloat(ServiceLocator<MicrophoneSinger>.Service.GetLoudnessFromMicrophone(), 2.8f);
            microphoneSignalBar.value = GetSignalFloat(ServiceLocator<MicrophoneSinger>.Service.GetLoudness(), 2.8f);
        }
    }

    private void SingerToggleValueChanged(bool newValue)
    {
        //singerIsPlayer.Value = newValue;
        gameStateScriptableObject.attackerIsPlayer = newValue;
    }

    private void DefenderToggleValueChanged(bool newValue)
    {
        //singerIsPlayer.Value = newValue;
        gameStateScriptableObject.defenderIsPlayer = newValue;
    }

    private void SingerErrorFrequencyChanged(float value)
    {
        //singerErrorFrequency.Value = value;
        gameStateScriptableObject.NPCSingerErrorFq = value;
    }

    private void DefenderPerfectionValueChanged(float value)
    {
        //singerErrorFrequency.Value = value;
        gameStateScriptableObject.defenderPerfectionLevel = value;
    }

}
