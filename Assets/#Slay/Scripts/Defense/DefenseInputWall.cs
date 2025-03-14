using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseInputWall : MonoBehaviour
{
    public Material highlightedMaterial;
    public GameStateScriptableObject gameState;

    public List<DefenseNoteObject> currentHighlightedNotes;

    [Header("Health")]
    public float healthRate = 1;
    public FloatReference defenderHealth;
    public GameEvent ShakeCameraEvent;

    //npc defender
    float time;
    float npcPlayWellInterval;

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
        for (int i = currentHighlightedNotes.Count - 1; i >= 0; i--)
        {
            //if (Input.GetKey(currentHighlightedNotes[i].keyCode))
            if (InputHandler.Instance.ListenToDefenderAction(currentHighlightedNotes[i].actionName))
            {
                currentHighlightedNotes[i].ReturnToPool();
                currentHighlightedNotes.RemoveAt(i);
            }
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
        defenderHealth.SetValueWithAlert(defenderHealth.Value - healthRate);
        ShakeCameraEvent.Raise();
    }
}
