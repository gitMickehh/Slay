using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;

public class DefenseInputWall : MonoBehaviour
{
    public Material highlightedMaterial;
    public GameStateScriptableObject gameState;

    public List<DefenseNoteObject> currentHighlightedNotes;

    [Header("Health")]
    public float healthRate = 1;
    public FloatReference player1Health;
    public FloatReference player2Health;
    
    [Header("Camera Shake")]
    public GameEvent ShakePlayer1CameraEvent;
    public GameEvent ShakePlayer2CameraEvent;

    //npc defender
    float time;
    float npcPlayWellInterval;

    [Header("Visual Feedback Boxes")]
    public Material notPressedMaterial;
    public Material pressedMaterial;
    public MeshRenderer topRight;
    public MeshRenderer topLeft;
    public MeshRenderer botRight;
    public MeshRenderer botLeft;

    private void Start()
    {
        currentHighlightedNotes = new List<DefenseNoteObject>();
        time = 0;
        npcPlayWellInterval = GetARandomInterval();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("trigger entered! " + other.name);

        if (other.tag == "defense-note")
        {
            //other.GetComponentInParent<DefenseNoteObject>().ReturnToPool();
            var currentNote = other.GetComponentInParent<DefenseNoteObject>();
            currentNote.SetMaterial(highlightedMaterial);
            currentHighlightedNotes.Add(currentNote);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "defense-note")
        {
            //hurt defender
            //Debug.Log("hurt defense!");
            HurtDefender();
            currentHighlightedNotes.Remove(other.GetComponentInParent<DefenseNoteObject>());
        }
    }

    private void Update()
    {
        if (gameState.defenderIsPlayer)
            CheckInputs();
        else
            NPCInputs();
    }

    private void CheckInputs()
    {
        LightUpVisualFeedback("");
        if(ServiceLocator<InputHandler>.Service.TopRightDown)
        {
            LightUpVisualFeedback("TopRight");
        }
        if (ServiceLocator<InputHandler>.Service.TopLeftDown)
        {
            LightUpVisualFeedback("TopLeft");
        }
        if (ServiceLocator<InputHandler>.Service.BotRightDown)
        {
            LightUpVisualFeedback("BotRight");
        }
        if (ServiceLocator<InputHandler>.Service.BotLeftDown)
        {
            LightUpVisualFeedback("BotLeft");
        }

        for (int i = currentHighlightedNotes.Count - 1; i >= 0; i--)
        {
            if (ServiceLocator<InputHandler>.Service.ListenToDefenderAction(currentHighlightedNotes[i].actionName))
            {
                currentHighlightedNotes[i].ReturnToPool();
                currentHighlightedNotes.RemoveAt(i);

                //LightUpVisualFeedback(currentHighlightedNotes[i].actionName);
            }
        }
    }

    private void LightUpVisualFeedback(string actionName)
    {
        switch (actionName)
        {
            case "TopRight":
                topRight.material = pressedMaterial;
                break;
            case "TopLeft":
                topLeft.material = pressedMaterial;
                break;
            case "BotRight":
                botRight.material = pressedMaterial;
                break;
            case "BotLeft":
                botLeft.material = pressedMaterial;
                break;
            default:
                topRight.material = notPressedMaterial;
                topLeft.material = notPressedMaterial;
                botRight.material = notPressedMaterial;
                botLeft.material = notPressedMaterial;
                break;
        }
    }

    private void DefendCurrentNotes()
    {
        for (int i = currentHighlightedNotes.Count - 1; i >= 0; i--)
        {
            currentHighlightedNotes[i].ReturnToPool();
            currentHighlightedNotes.RemoveAt(i);
        }
    }

    private void NPCInputs()
    {
        if (gameState.defenderPerfectionLevel >= 1.0f)
        {
            DefendCurrentNotes();
        }
        else if(gameState.defenderPerfectionLevel > 0.0f)
        {
            //set a timer to hit the right notes
            time += Time.deltaTime;
            if(time >= npcPlayWellInterval)
            {
                time = 0;
                npcPlayWellInterval = GetARandomInterval();
                DefendCurrentNotes();
            }
        }
    }

    private float GetARandomInterval()
    {
        var r = Random.Range(0.0f, gameState.defenderPerfectionLevel);
        r = (1.0f - r) * 2.0f;

        return r;
    }

    private void HurtDefender()
    {
        if(!gameState.switchedRoles)
        {
            player2Health.SetValueWithAlert(player2Health.Value - healthRate);
            ShakePlayer2CameraEvent.Raise();
        }
        else
        {
            player1Health.SetValueWithAlert(player1Health.Value - healthRate);
            ShakePlayer1CameraEvent.Raise();
        }
    }
}
