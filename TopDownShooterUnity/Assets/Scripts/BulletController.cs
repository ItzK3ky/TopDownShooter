using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Attributes")]
    [SerializeField] private float despawnTimeInSeconds;

    void Update()
    {
            Destroy(transform.gameObject, despawnTimeInSeconds);
    }
}