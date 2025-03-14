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


    private InputAction topRightAction;
    private InputAction topLeftAction;
    private InputAction botRightAction;
    private InputAction botLeftAction;

    private InputAction backAction;


    public bool TopRightTriggered { get; private set; }
    public bool TopLeftTriggered { get; private set; }
    public bool BotRightTriggered { get; private set; }
    public bool BotLeftTriggered { get; private set; }
    public bool BackTriggered { get; private set; }

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
        RegisterInputActions();
    }

    public bool ListenToDefenderAction(string actionName)
    {
        return controlsAsset.FindActionMap(defenderActionMapName).FindAction(actionName).IsPressed();
    }

    void RegisterInputActions()
    {
        topRightAction.performed += context => TopRightTriggered = true;
        topRightAction.canceled += context => TopRightTriggered = false;

        topLeftAction.performed += context => TopLeftTriggered = true;
        topLeftAction.canceled += context => TopLeftTriggered = false;

        botRightAction.performed += context => BotRightTriggered = true;
        botRightAction.canceled += context => BotRightTriggered = false;

        botLeftAction.performed += context => BotLeftTriggered = true;
        botLeftAction.canceled += context => BotLeftTriggered = false;

        backAction.performed += context => BackTriggered = true;
        backAction.canceled += context => BackTriggered = false;
    }

    private void OnEnable()
    {
        topRightAction.Enable();
        topLeftAction.Enable();
        botRightAction.Enable();
        botLeftAction.Enable();
        backAction.Enable();
    }

    private void OnDisable()
    {
        topRightAction.Disable();
        topLeftAction.Disable();
        botRightAction.Disable();
        botLeftAction.Disable();
        backAction.Disable();
    }

}
