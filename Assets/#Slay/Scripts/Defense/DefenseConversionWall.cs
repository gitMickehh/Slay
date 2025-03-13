using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseConversionWall : MonoBehaviour
{
    public DefenseConvertor convertor;
    public float velocityMultiplyer = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name + " has entered!");
        if (other.tag == "singing_note")
        {
            //var transformMaterial = convertor.GetRandomPosition();
            //other.transform.parent.position = transformMaterial.transform.position;
            //other.GetComponentInParent<MidiNoteObject>().SetOutsideMaterial(transformMaterial.material);

            convertor.GenerateDefenseNote(other.GetComponentInParent<MidiNoteObject>().rbody.velocity * velocityMultiplyer);
            other.GetComponentInParent<MidiNoteObject>().ReturnToPool();
        }
    }
}
