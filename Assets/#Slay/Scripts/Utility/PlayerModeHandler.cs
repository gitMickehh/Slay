using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerModeHandler : MonoBehaviour
{
    public GameStateScriptableObject gameStateScriptableObject;
    public FloatReference currentTrackPosition;

    [Header("Player 1")]
    public Camera player1Camera;
    public Transform singerCameraTransform;

    [Header("Player 2")]
    public Camera player2Camera;
    public Transform defenderCameraTransform;

    [Header("End Screen")]
    public UIDocument EndScreenUIDocument;

    private Queue<float> switchTimes;
    private bool switched;
    private float currentTimeGoal;
    private bool noMoreSwitches;
    private Label switchText;

    //end screen
    private Label endScreenText;
    private Button endScreenButton;

    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        switchText = root.Q<Label>("switchLabel");

        noMoreSwitches = false;
        switched = false;
        gameStateScriptableObject.switchedRoles = switched;
        switchTimes = new Queue<float>();
        foreach (var switchTime in gameStateScriptableObject.currentSong.Value.breaktimes) {
            switchTimes.Enqueue(switchTime);
        }
        currentTimeGoal = switchTimes.Dequeue();
        switchText.text = "";
        currentTrackPosition.Value = 0;

        VisualElement endScreenRoot = EndScreenUIDocument.rootVisualElement;
        endScreenText = endScreenRoot.Q<Label>("endscreenText");
        endScreenButton = endScreenRoot.Q<Button>("playagain");
        endScreenButton.clicked += GoToMainMenu;

        EndScreenUIDocument.gameObject.SetActive(false);

        UpdateCameraPosition();
    }

    private void Update()
    {
        CheckSongEnded();

        if (noMoreSwitches) return;

        if(currentTrackPosition.Value >= currentTimeGoal)
        {
            //switch
            switched = !switched;
            gameStateScriptableObject.switchedRoles = switched;

            UpdateCameraPosition();
            currentTimeGoal = switchTimes.Dequeue();
            switchText.text = "";

            if (switchTimes.Count == 0)
                noMoreSwitches = true;
        }
        else if (currentTrackPosition.Value >= currentTimeGoal - 3.5f)
        {
            //UI count down!
            switchText.text = "Switch in " + Mathf.CeilToInt(currentTimeGoal - currentTrackPosition.Value) + "..";
        }
    }

    private void UpdateCameraPosition()
    {
        player1Camera.transform.SetParent(switched? defenderCameraTransform: singerCameraTransform);
        player1Camera.transform.localPosition = Vector3.zero;
        player1Camera.transform.localRotation = Quaternion.identity;

        player2Camera.transform.SetParent(switched ? singerCameraTransform: defenderCameraTransform);
        player2Camera.transform.localPosition = Vector3.zero;
        player2Camera.transform.localRotation = Quaternion.identity;
    }

    private void CheckSongEnded()
    {
        if(currentTrackPosition.Value >= gameStateScriptableObject.currentSong.Value.instrumental.length)
        {
            //end game
            EndScreenUIDocument.gameObject.SetActive(true);

            bool player1IsWinner = gameStateScriptableObject.player1Health.Value > gameStateScriptableObject.player2Health.Value;
            bool draw = gameStateScriptableObject.player1Health.Value == gameStateScriptableObject.player2Health.Value;

            if (draw)
            {
                endScreenText.text = "Wow!! It's a draw!!! Slay Again and this time make sure someone wins!!";
                return;
            }

            var newString = endScreenText.text.Replace('#', player1IsWinner? '1': '2');
            endScreenText.text = newString;
        }
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
