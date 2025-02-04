using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class GameObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private Queue<GameObject> objects = new Queue<GameObject>();

    private Vector3 startingPosition;

    public void SetStartingPosition(Vector3 newStartingPosition)
    {
        startingPosition = newStartingPosition;
    }

    public GameObject Get()
    {
        if(objects.Count == 0)
        {
            AddObjects(1);
        }

        var returning = objects.Dequeue();
        returning.SetActive(true);

        return returning;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        if (objects.Contains(objectToReturn))
            return;

        objects.Enqueue(objectToReturn);
        objectToReturn.SetActive(false);
    }

    private void AddObjects(int count)
    {
        if(startingPosition == null)
        {
            startingPosition = Vector3.zero;
        }

        for (int i = 0; i < count; i++)
        {
            var newObject = Instantiate(prefab, startingPosition, Quaternion.identity);
            newObject.GetComponent<IObjectPooled>().Pool = this;
            newObject.SetActive(false);
            objects.Enqueue(newObject);

            newObject.transform.SetParent(transform);
            //Debug.Log("Created a new object for the pool.");
        }
    }

    public void ChangePoolObjects(GameObject newObject)
    {
        if (newObject == prefab)
            return;

        while (objects.Count != 0)
        {
            Destroy(objects.Dequeue());
        }

        prefab = newObject;
    }

    public bool CheckPoolObject(GameObject objectToCheck)
    {
        return (objectToCheck == prefab);
    }
}
