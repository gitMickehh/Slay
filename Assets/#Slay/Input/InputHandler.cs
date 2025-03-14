using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset controlsAsset;

    [Header("Action Map Names")]
    [SerializeField] private string defenderActionMapName;
    [SerializeField] private string controlActionMapName;

    [Header("Action Map Names")]
    [SerializeField] private string topRight = "TopRight";
    [SerializeField] private string topLeft = "TopLeft";
    [SerializeField] private string botRight = "BotRight";
    [SerializeField] private string botLeft = "BotLeft";
    
    [SerializeField] private string back = "Back";
    [SerializeField] private string switchName = "Switch";


    private InputAction topRightAction;
    private InputAction topLeftAction;
    private InputAction botRightAction;
    private InputAction botLeftAction;

    private InputAction backAction;
    private InputAction switchAction;


    public bool TopRightTriggered { get => topRightAction.WasPressedThisFrame(); }
    public bool TopLeftTriggered { get => topLeftAction.WasPressedThisFrame(); }
    public bool BotRightTriggered { get => botRightAction.WasPressedThisFrame(); }
    public bool BotLeftTriggered { get => botLeftAction.WasPressedThisFrame(); }
    public bool BackTriggered { get => backAction.WasPressedThisFrame(); }
    public bool SwitchCameraTrigger { get => switchAction.WasPressedThisFrame(); }

    public static InputHandler Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        topRightAction = controlsAsset.FindActionMap(defenderActionMapName).FindAction(topRight);
        topLeftAction = controlsAsset.FindActionMap(defenderActionMapName).FindAction(topLeft);
        botRightAction = controlsAsset.FindActionMap(defenderActionMapName).FindAction(botRight);
        botLeftAction = controlsAsset.FindActionMap(defenderActionMapName).FindAction(botLeft);

        backAction = controlsAsset.FindActionMap(controlActionMapName).FindAction(back);
        switchAction = controlsAsset.FindActionMap(controlActionMapName).FindAction(switchName);
    }

    public bool ListenToDefenderAction(string actionName)
    {
        return controlsAsset.FindActionMap(defenderActionMapName).FindAction(actionName).IsPressed();
    }

    private void OnEnable()
    {
        topRightAction.Enable();
        topLeftAction.Enable();
        botRightAction.Enable();
        botLeftAction.Enable();
        backAction.Enable();
        switchAction.Enable();
    }

    private void OnDisable()
    {
        topRightAction.Disable();
        topLeftAction.Disable();
        botRightAction.Disable();
        botLeftAction.Disable();
        backAction.Disable();
        switchAction.Disable();
    }

}
