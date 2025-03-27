using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MPBar : MonoBehaviour
{
    [SerializeField] private MP playerMP;
    [SerializeField] private Image totalMPBar;
    [SerializeField] private Image CurrentMPBar;
    [SerializeField] private TextMeshProUGUI textMP;

    private void Start()
    {
        totalMPBar.fillAmount = playerMP.currentMP / playerMP.getMaxMP();
        textMP.text = playerMP.currentMP + "/" + playerMP.getMaxMP();
    }

    private void Update()
    {
        totalMPBar.fillAmount = playerMP.currentMP / playerMP.getMaxMP();
        textMP.text = playerMP.currentMP + "/" + playerMP.getMaxMP();
    }
}
