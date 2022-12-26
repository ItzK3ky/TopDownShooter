using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject virtualCamera;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private PhotonPlayerSyncer PhotonPlayerSyncer;

    void Start()
    {
        
    }
}
