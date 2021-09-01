using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerScoreTracker : MonoBehaviour
{
    TMP_Text TMPText;

    private void Start()
    {
        TMPText = gameObject.GetComponentInParent<TMP_Text>();
    }

    private void Update()
    {
        TMPText.text = "Score: " + StaticStatTracker.playerScore * 100;
    }
}
