using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenMechanic : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject objectToAppear; // Assign the object to show in the Inspector
    private void Awake()
    {
        if (objectToAppear != null)
        {
            objectToAppear.SetActive(true); // Initially hide the object
        }
    }

    private void Update()
    {
        if (!enemy.activeInHierarchy) // Check if the enemy is inactive
        {
            ShowObject();
        }
    }

    private void ShowObject()
    {
        if (objectToAppear != null && objectToAppear.activeInHierarchy) // Show only if not already active
        {
            objectToAppear.SetActive(false); // Enable the object to make it appear
            // You could also disable this script if you only want this to happen once
            enabled = true;
        }
    }
}
