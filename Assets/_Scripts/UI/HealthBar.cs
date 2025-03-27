using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image CurrentHealthBar;
    [SerializeField] private TextMeshProUGUI textHP;

    private void Start()
    {
        totalHealthBar.fillAmount = playerHealth.currentHealth/playerHealth.getMaxHealth();
        textHP.text = playerHealth.currentHealth + "/" + playerHealth.getMaxHealth();
    }

    private void Update()
    {
        totalHealthBar.fillAmount = playerHealth.currentHealth / playerHealth.getMaxHealth();
        textHP.text = playerHealth.currentHealth + "/" + playerHealth.getMaxHealth();
    }
}
