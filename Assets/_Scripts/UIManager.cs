using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreDisplay;
    [SerializeField] private TextMeshProUGUI _countDisplay;
    [SerializeField] private Image _pv;


    public void UpdateScore(int score)
    {
        _scoreDisplay.text = score.ToString();
    }

    public void UpdatePV(int pv)
    {
        _pv.fillAmount = (float)pv / 30f;
    }

    public void UpdateBallCount(int count)
    {
        _countDisplay.text = count.ToString();
    }
}
