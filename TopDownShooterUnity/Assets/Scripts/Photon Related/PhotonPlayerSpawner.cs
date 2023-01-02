using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonPlayerSpawner : MonoBehaviour
{
    public static PhotonPlayerSpawner Instance { get; private set; }

    #region Script Description for inspector

    [Header("   SCRIPT DESCRIPTION:   ")]
    [Header("This script takes care of spawning the \n" +
            "player in (on the \"Photon network\")")]
    [Space(20)]

    [SerializeField] private bool iDoNothingLol;

    [Space(20)]

    #endregion

    [SerializeField] private GameObject _playerPrefab;

    [HideInInspector] public GameObject playerObjectOfThisClient;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start() => StartCoroutine(SpawnPlayer());

    /// <summary>
    /// Spawns player on on Photon Room (so it's visible for all players)
    /// and  assigns an playerIndex to the player
    /// </summary>
    private IEnumerator SpawnPlayer()
    {
        yield return new WaitForSecondsRealtime(2f);

        playerObjectOfThisClient = PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity);
        int thisClientsPlayerIndex = playerObjectOfThisClient.GetComponent<PlayerController>().playerIndex = PhotonNetwork.PlayerList.Length - 1;

        //Send this clients player object to all other clients
        PhotonPlayerManager.Instance.photonView.RPC("setMostRecentPlayerToJoin", RpcTarget.Others, playerObjectOfThisClient.GetPhotonView().ViewID);

        //Add "myself" to objects to be synced
        PhotonPlayerSyncer.Instance.AddPlayerObjectToSync(playerObjectOfThisClient, thisClientsPlayerIndex);
    }
}
