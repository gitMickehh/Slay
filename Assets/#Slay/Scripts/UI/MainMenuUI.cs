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

    //[Header("Songs")]
    //public Song britneySong;
    //public Song charlieSong;
    //public Song MileySong;
    //public Song PPCSong;

    public List<Song> songs;

    DropdownField microphonesDropDownField;
    ProgressBar microphoneSignalBar;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        //Button buttonBritney = root.Q<Button>("ButtonBritney");
        //Button buttonCharlie = root.Q<Button>("ButtonCharlie");
        //Button buttonMiley = root.Q<Button>("ButtonMiley");
        //Button buttonChapel = root.Q<Button>("ButtonChapel");
        //buttonBritney.clicked += OnClickBritney;
        //buttonCharlie.clicked += OnClickCharlie;
        //buttonMiley.clicked += OnClickMiley;
        //buttonChapel.clicked += OnClickPPC;

        DropdownField songDropDownField = root.Q<DropdownField>("SongChoice");
        songDropDownField.choices = GetSongsAsList();
        songDropDownField.RegisterValueChangedCallback(evt => SetSongByString(evt.newValue));
        songDropDownField.index = 0;

        microphonesDropDownField = root.Q<DropdownField>("microphoneDropDown");
        microphonesDropDownField.choices = Microphone.devices.ToList<string>();
        //microphonesDropDownField.index = 0;
        //microphonesDropDownField.RegisterValueChangedCallback(evt => SetMicrophoneName(evt.newValue));

        microphoneSignalBar = root.Q<ProgressBar>("audioSignal");
        microphoneSignalBar.value = 0;

        Button startButton = root.Q<Button>("startButton");
        startButton.clicked += StartNextScene;

        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        microphonesDropDownField.RegisterValueChangedCallback(evt => SetMicrophoneName(evt.newValue));
        microphonesDropDownField.index = 0;
        ServiceLocator<MicrophoneManager>.Service.StartSinger();
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
        microphoneName.Value = microphoneDeviceName;
    }

    private void StartNextScene()
    {
        SceneManager.LoadScene(1);
    }

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

            microphoneSignalBar.value = GetSignalFloat(ServiceLocator<MicrophoneManager>.Service.GetLoudnessFromMicrophone(), 2.8f);
        }
    }
}
