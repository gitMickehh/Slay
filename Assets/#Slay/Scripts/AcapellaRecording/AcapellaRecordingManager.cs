using Slay;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AcapellaRecordingManager : MonoBehaviour
{
    [Header("Scriptable References")]
    [SerializeField]
    private SongReference m_SongReference;

    [SerializeField]
    private AcapellaController acapellaController;


    private void Awake()
    {
        if (!File.Exists(m_SongReference.Value.SongPath))
        {
            Debug.Log("File at" + m_SongReference.Value.SongPath + " doesn't exist, going to start the process of making one.");
            Instantiate(acapellaController);
        }
        else
        {
            Debug.Log("File at" + m_SongReference.Value.SongPath + " exists");
        }
    }
}
