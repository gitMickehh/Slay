using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DefenseNoteObject : MonoBehaviour, IObjectPooled
{
    public GameObjectPool Pool { get; set; }

    public MeshRenderer noteModel;
    public Rigidbody rbody;
    public KeyCode keyCode;
    public TextMeshProUGUI keyCodeText;

    public void SetVelocity(Vector3 velocity)
    {
        rbody.velocity = velocity;
    }

    public void SetMaterial(Material material)
    {
        noteModel.material = material;
    }

    public void SetKeyCode(KeyCode newKeyCode)
    {
        keyCode = newKeyCode;
        keyCodeText.text = newKeyCode.ToString();
    }

    public void ReturnToPool()
    {
        rbody.velocity = Vector3.zero;
        Pool.ReturnToPool(gameObject);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pool_Reset_Box")
        {
            ReturnToPool();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pool_Reset_Box")
        {
            ReturnToPool();
        }
    }
}
