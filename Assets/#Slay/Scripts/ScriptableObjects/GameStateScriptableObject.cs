using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game State Object", menuName = "Slay/Game State Object")]
public class GameStateScriptableObject : ScriptableObject
{
    [Header("Attacker")]
    public bool attackerIsPlayer;

    [Range(0f,5.0f)]
    public float NPCSingerErrorFq = 0.35f;

    [Header("Defender")]
    public bool defenderIsPlayer;
    [Range(0f,1.0f)]
    public float defenderPerfectionLevel = 1.0f;

}
