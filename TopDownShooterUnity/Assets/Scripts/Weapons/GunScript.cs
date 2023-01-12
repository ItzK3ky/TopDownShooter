using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GunScript : MonoBehaviour
{
    #region Script Description for inspector

    [Header("   SCRIPT DESCRIPTION:   ")]
    [Header("This script takes care of shooting the \n" +
            "gun")]
    [Space(20)]

    [SerializeField] private bool iDoNothingLol;

    [Space(20)]

    #endregion

    [Header("Gun Attributes")]
    [SerializeField] private float shootingCooldownInSeconds;
    [SerializeField] private float bulletSpeed;

    [Header("Objects")]
    [SerializeField] private GameObject gunBarrelEnd;
    [SerializeField] private GameObject bulletPrefab;

    private PhotonView _view;

    private float cooldown;

    private void Start()
    {
        _view = GetComponent<PhotonView>();
    }

    void Update()
    {

        if (!_view.IsMine)
            return;

        cooldown -= Time.deltaTime;

        if (cooldown <= 0)
        {
            if (Input.GetMouseButton(0))
            {
                GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, gunBarrelEnd.transform.position, gunBarrelEnd.transform.rotation);

                BulletScript bulletScript = bullet.GetComponent<BulletScript>();
                bulletScript.shootBullet(bulletSpeed, gunBarrelEnd.transform.up);
                

                cooldown = shootingCooldownInSeconds;
            }
        }
    }
}
