using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MidiNoteObject : MonoBehaviour, IObjectPooled
{
    public MidiNoteTimeStamped midiNote;
    public bool isAtMidPoint;

    [Header("Mesh Look")]
    public MeshRenderer note_mesh;
    public Material neutral_material;
    public Material selected_material;
    public Material on_hit_material;
    public Material on_almost_hit_material;
    public Material on_miss_material;

    [Header("Look Settings")]
    public float lengthMultiplyer = 1.0f;

    [Header("Shooting")]
    public Rigidbody rbody;
    public Collider note_collider;
    public float projectileForce;

    private double timeInstantiated;
    private float noteLength = 1;

    public float originalScaleX = -0.1f;
    private bool flying;

    public GameObjectPool Pool { get; set; }

    public void SetupNote(Vector3 position, double currentAudioTime, MidiNoteTimeStamped timeStampedNote)
    {
        transform.localPosition = position;
        isAtMidPoint = false;
        timeInstantiated = currentAudioTime;
        midiNote = timeStampedNote;

        name = "[" + timeStampedNote.note.NoteName + timeStampedNote.note.Octave + "]";

        flying = false;
        isAtMidPoint = false;

        SetLength((float)timeStampedNote.noteLength * lengthMultiplyer);
        SetNeutralMaterial();
    }

    public void UpdateNote(double currentAudioTime, float noteTime, float spawnPoint_X, float despawnPoint_X)
    {
        if (!flying)
        {
            double timeSinceInstantiated = currentAudioTime - timeInstantiated;
            float t = (float)(timeSinceInstantiated / (noteTime * 2f));

            if (t > 1)
            {
                ReturnToPool();
            }
            else
            {
                var xLerp = Mathf.Lerp(spawnPoint_X, despawnPoint_X, t);
                transform.localPosition = new Vector3(xLerp, transform.localPosition.y, transform.localPosition.z);
            }
        }
    }

    public void SetNeutralMaterial()
    {
        note_mesh.material = neutral_material;
    }

    public void SetSelectedMaterial()
    {
        note_mesh.material = selected_material;
    }

    public void Hit(bool exact)
    {
        flying = true;

        var force = projectileForce;
        if (exact)
        {
            SetCorrectMaterial();
            //rbody.angularVelocity = Vector3.up * force;
        }
        else
        {
            SetAlmostCorrectMaterial();
            force = force * 0.7f;
        }

        //then shoot off
        //rbody.isKinematic = false;
        //rbody.useGravity = true;

        //rbody.AddForce(Vector3.back * force, ForceMode.Impulse);
        rbody.velocity = Vector3.back * force;
    }

    public void Miss()
    {
        //flying = true;

        SetIncorrectMaterial();
        //make it fail
    }

    private void SetCorrectMaterial()
    {
        note_mesh.material = on_hit_material;
    }

    private void SetAlmostCorrectMaterial()
    {
        note_mesh.material = on_almost_hit_material;
    }

    private void SetIncorrectMaterial()
    {
        note_mesh.material = on_miss_material;
    }

    private void SetLength(float length)
    {
        noteLength = length;
        //transform.localScale = new Vector3(noteLength * transform.localScale.x * (-1), transform.localScale.y, transform.localScale.z);
        transform.localScale = new Vector3(noteLength * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.tag == "midpoint")
        //{
        //    HighlightNote();
        //}
        if (other.tag == "Pool_Reset_Box")
        {
            ReturnToPool();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (!flying)
        //{
        //    UnHighlightNote();
        //}

        if (other.tag == "Pool_Reset_Box")
        {
            ReturnToPool();
        }
    }

    public void SetOutsideMaterial(Material material)
    {
        note_mesh.material = material;
    }

    public void HighlightNote()
    {
        isAtMidPoint = true;
        SetSelectedMaterial();
    }

    public void UnHighlightNote()
    {
        isAtMidPoint = false;
        SetNeutralMaterial();
    }

    private void ResetObject()
    {
        //Debug.Log(name + " is reset.");

        transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        isAtMidPoint = false;
        flying = false;
        rbody.velocity = Vector3.zero;
        rbody.angularVelocity = Vector3.zero; 

        SetNeutralMaterial();
    }

    public void ReturnToPool()
    {
        ResetObject();
        Pool.ReturnToPool(gameObject);
        gameObject.SetActive(false);
    }
}
