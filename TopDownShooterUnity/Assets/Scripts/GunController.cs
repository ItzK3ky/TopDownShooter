using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Gun Attributes")]
    [SerializeField] private float shootingCooldown;
    [SerializeField] private float bulletSpeed;

    [Header("Objects")]
    [SerializeField] private GameObject gunBarrelEnd;
    [SerializeField] private GameObject bulletPrefab;

    private float cooldown;

    private void Start()
    {
        cooldown = shootingCooldown;
    }

    void Update()
    {
        cooldown -= Time.deltaTime;

        if (cooldown <= 0)
        {
            if (Input.GetMouseButton(0))
            {
                Transform spawnTransform = gunBarrelEnd.transform;

                GameObject bullet = Instantiate(bulletPrefab, spawnTransform.position, spawnTransform.rotation);

                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                bulletRb.AddForce(gunBarrelEnd.transform.up * bulletSpeed, ForceMode2D.Impulse);

                cooldown = shootingCooldown;
            }
        }
    }
}
