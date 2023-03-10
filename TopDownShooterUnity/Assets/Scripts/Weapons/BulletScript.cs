using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletScript : MonoBehaviour
{
    #region Script Description for inspector

    [Header("   SCRIPT DESCRIPTION:   ")]
    [Header("This script takes care of moving the bullet \n" +
            "when shot")]
    [Space(20)]

    [SerializeField] private bool iDoNothingLol;

    [Space(20)]

    #endregion

    [Header("Bullet Attributes")]
    [SerializeField] private float despawnTimeInSeconds;

    private PhotonView _view;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _view = GetComponent<PhotonView>();        
    }

    public void shootBullet(float bulletSpeed, Vector3 gunBarrelEndTransformUp)
    {
        _view.RPC("shootBulletRPC", RpcTarget.All, bulletSpeed, gunBarrelEndTransformUp);
    }

    private IEnumerator destroyBulletAfterSeconds(float despawnTime)
    {
        yield return new WaitForSecondsRealtime(despawnTime);
        PhotonNetwork.Destroy(_view);
    }

    [PunRPC]
    private void shootBulletRPC(float bulletSpeed, Vector3 gunBarrelEndTransformUp)
    {
        _rb.AddForce(gunBarrelEndTransformUp * bulletSpeed, ForceMode2D.Impulse);

        StartCoroutine(destroyBulletAfterSeconds(despawnTimeInSeconds));
    }
}