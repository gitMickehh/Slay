using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseInputWall : MonoBehaviour
{
    public Material highlightedMaterial;

    public List<DefenseNoteObject> currentHighlightedNotes;

    private void Start()
    {
        currentHighlightedNotes = new List<DefenseNoteObject>();
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
            Debug.Log("hurt defense!");
            currentHighlightedNotes.Remove(other.GetComponentInParent<DefenseNoteObject>());
        }
    }

    private void Update()
    {
        CheckInputs();
    }

    private void CheckInputs()
    {
        for (int i = currentHighlightedNotes.Count-1; i >= 0 ; i--)
        {
            if (Input.GetKey(currentHighlightedNotes[i].keyCode))
            {
                currentHighlightedNotes[i].ReturnToPool();
                currentHighlightedNotes.RemoveAt(i);
            }
        }
    }
}
