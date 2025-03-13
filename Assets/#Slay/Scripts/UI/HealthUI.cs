using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthUI : MonoBehaviour
{
    public FloatReference attackerHealth;
    public FloatReference defenderHealth;

    ProgressBar attackerHealthMeter;
    ProgressBar defenderHealthMeter;

    private void OnEnable()
    {
        attackerHealth.onChangedAction += OnAttackerHealthChanged;
        defenderHealth.onChangedAction += OnDefenderHealthChanged;

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        attackerHealthMeter = root.Q<ProgressBar>("attackerHealthMeter");
        defenderHealthMeter = root.Q<ProgressBar>("defenderHealthMeter");

        attackerHealthMeter.value = attackerHealth.Value;
        attackerHealthMeter.highValue = attackerHealth.Value;
        attackerHealthMeter.title = attackerHealth.Value.ToString();

        defenderHealthMeter.value = defenderHealth.Value;
        defenderHealthMeter.highValue= defenderHealth.Value;
        defenderHealthMeter.title = defenderHealth.Value.ToString();
    }

    private void OnAttackerHealthChanged()
    {
        attackerHealthMeter.value = attackerHealth.Value;
        attackerHealthMeter.title = attackerHealth.Value.ToString();
    }

    private void OnDefenderHealthChanged()
    {
        defenderHealthMeter.value = defenderHealth.Value;
        defenderHealthMeter.title = defenderHealth.Value.ToString();
    }
}
