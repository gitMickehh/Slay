using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    public SongReference songReference;

    public Song britneySong;
    public Song chapelSong;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button buttonBritney = root.Q<Button>("ButtonBritney");
        Button buttonChapel = root.Q<Button>("ButtonChapel");

        buttonBritney.clicked += OnClickBritney;
        buttonChapel.clicked += OnClickChapel;
    }

    private void OnClickBritney()
    {
        SetSongReference(britneySong);
        StartNextScene();
    }

    private void OnClickChapel()
    {
        SetSongReference(chapelSong);
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
