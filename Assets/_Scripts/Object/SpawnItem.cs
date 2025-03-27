using Cainos.PixelArtPlatformer_VillageProps;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private GameObject[] items; 

    [Header("Chest")]
    [SerializeField] private Chest chest;

    private Boolean isSpawn = false;

    private void Update()
    {
        if (chest.IsOpened && !isSpawn)
        {
            foreach (GameObject item in items) 
            {
                GameObject itemInstance = Instantiate(item, transform.position, Quaternion.identity);
                itemInstance.SetActive(true);
            }

            isSpawn = true; 
        }
    }
}

