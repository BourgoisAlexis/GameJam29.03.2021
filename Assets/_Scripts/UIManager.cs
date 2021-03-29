using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreDisplay;

    public void UpdateScore(int score)
    {
        scoreDisplay.text = score.ToString();
    }
}
