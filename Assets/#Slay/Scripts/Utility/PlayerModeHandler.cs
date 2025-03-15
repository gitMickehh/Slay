using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;
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


    private Queue<float> switchTimes;
    private bool switched;
    private float currentTimeGoal;
    private bool noMoreSwitches;
    private Label switchText;

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

        UpdateCameraPosition();
    }

    private void Update()
    {
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
}
