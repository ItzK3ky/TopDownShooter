using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeRotationForChild : MonoBehaviour
{
    void Update()
    {
        transform.localRotation = Quaternion.Inverse(transform.parent.rotation);
    }
}
    