using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class TransformMaterial
{
    public Transform transform;
    public Material material;
}

public class DefenseConvertor : MonoBehaviour
{
    //public List<Transform> possibleSpawnPositions;
    public List<TransformMaterial> possibleSpawnPositions;
    private int randomIndex;

    private void Start()
    {
        randomIndex = -1;
    }

    private TransformMaterial GetRandomPosition(int lastRandomIndex)
    {
        //List<Transform> newPossibleSpawnPositions = possibleSpawnPositions;
        List<TransformMaterial> newPossibleSpawnPositions = possibleSpawnPositions;

        if (lastRandomIndex != -1)
            newPossibleSpawnPositions = possibleSpawnPositions.ToList().Where(p => p.transform.position != possibleSpawnPositions[lastRandomIndex].transform.position).ToList();
            //newPossibleSpawnPositions = possibleSpawnPositions.ToList().Where(p => p.position != possibleSpawnPositions[lastRandomIndex].position).ToList();

        randomIndex = Random.Range(0,newPossibleSpawnPositions.Count);
        return newPossibleSpawnPositions[randomIndex];
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + " has entered!");
        if (other.tag == "singing_note")
        {
            var transformMaterial = GetRandomPosition(randomIndex);
            other.transform.parent.position = transformMaterial.transform.position;
            other.GetComponentInParent<MidiNoteObject>().SetOutsideMaterial(transformMaterial.material);
        }
    }

}
