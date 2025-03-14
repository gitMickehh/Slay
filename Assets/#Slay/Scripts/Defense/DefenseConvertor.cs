using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class TransformMaterial
{
    public Transform transform;
    public Material material;
    //public KeyCode keyCode;
    public string actionName;
    public string titleName;
}

public class DefenseConvertor : MonoBehaviour
{
    //public List<Transform> possibleSpawnPositions;
    public List<TransformMaterial> possibleSpawnPositions;
    private int randomIndex;
    //public GameObject DefenderNote;
    public GameObjectPool defenderNotePool;

    private void Start()
    {
        randomIndex = -1;
    }

    private TransformMaterial GetRandomPosition()
    {
        //List<Transform> newPossibleSpawnPositions = possibleSpawnPositions;
        List<TransformMaterial> newPossibleSpawnPositions = possibleSpawnPositions;

        if (randomIndex != -1)
            newPossibleSpawnPositions = possibleSpawnPositions.ToList().Where(p => p.transform.position != possibleSpawnPositions[randomIndex].transform.position).ToList();
            //newPossibleSpawnPositions = possibleSpawnPositions.ToList().Where(p => p.position != possibleSpawnPositions[lastRandomIndex].position).ToList();

        randomIndex = Random.Range(0,newPossibleSpawnPositions.Count);
        return newPossibleSpawnPositions[randomIndex];
    }

    public void GenerateDefenseNote(Vector3 velocity)
    {
        var transformMaterial = GetRandomPosition();
        DefenseNoteObject newDefenseNote = defenderNotePool.Get().GetComponent<DefenseNoteObject>();

        newDefenseNote.transform.position = transformMaterial.transform.position;
        newDefenseNote.SetVelocity(velocity);
        newDefenseNote.SetMaterial(transformMaterial.material);
        //newDefenseNote.SetKeyCode(transformMaterial.keyCode);
        newDefenseNote.SetActionName(transformMaterial.actionName, transformMaterial.titleName);
    }



}
