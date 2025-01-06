using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    public SongReference songReference;

    [Header("Songs")]
    public Song britneySong;
    public Song charlieSong;
    public Song MileySong;
    public Song PPCSong;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button buttonBritney = root.Q<Button>("ButtonBritney");
        Button buttonCharlie = root.Q<Button>("ButtonCharlie");
        Button buttonMiley = root.Q<Button>("ButtonMiley");
        Button buttonChapel = root.Q<Button>("ButtonChapel");

        buttonBritney.clicked += OnClickBritney;
        buttonCharlie.clicked += OnClickCharlie;
        buttonMiley.clicked += OnClickMiley;
        buttonChapel.clicked += OnClickPPC;
    }

    private void OnClickBritney()
    {
        SetSongReference(britneySong);
        StartNextScene();
    }

    private void OnClickCharlie()
    {
        SetSongReference(charlieSong);
        StartNextScene();
    }

    private void OnClickPPC()
    {
        SetSongReference(PPCSong);
        StartNextScene();
    }

    private void OnClickMiley()
    {
        SetSongReference(MileySong);
        StartNextScene();
    }

    private void SetSongReference(Song song)
    {
        songReference.Value = song;
    }

    private void StartNextScene()
    {
        SceneManager.LoadScene(1);
    }
}
