using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealthTracker : MonoBehaviour
{
    TMP_Text TMPText;

    private void Start()
    {
        TMPText = gameObject.GetComponentInParent<TMP_Text>();
    }

    private void Update()
    {
        float healthPercent = StaticStatTracker.playerHealth / StaticStatTracker.playerMaxHealth * 100;
        TMPText.text = "Health: " + healthPercent + "%";
    }
}
