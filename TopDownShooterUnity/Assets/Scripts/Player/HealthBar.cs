using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform healthBar;
    [SerializeField] private PlayerController playerController;

    void Update()
    {
        float playerHealth = playerController.health;

        healthBar.localScale = new Vector3(playerHealth / 100, 1, 1);
    }
}
