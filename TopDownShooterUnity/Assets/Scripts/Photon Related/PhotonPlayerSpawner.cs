using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    private PhotonView _view;

    void Awake()
    {
        _view = GetComponent<PhotonView>();

        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        StartCoroutine(SpawnPlayer());
    }

    /// <summary>
    /// Spawns player on on Photon Room (so it's visible for all players)
    /// and  assigns an playerIndex to the player
    /// </summary>
    private IEnumerator SpawnPlayer()
    {
        yield return new WaitForSecondsRealtime(2f);

        playerObjectOfThisClient = PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity);

        //Send this clients player object to all other clients
        PhotonPlayerManager.Instance.photonView.RPC("setMostRecentPlayerToJoin", RpcTarget.Others, playerObjectOfThisClient.GetPhotonView().ViewID);

        //Change Nickname text to Nickname
        playerObjectOfThisClient.GetComponentInChildren<TMP_Text>().text = PhotonNetwork.LocalPlayer.NickName;

        //Set "FinishedSpawning" property
        Hashtable finishedSpawningHash = new Hashtable();
        finishedSpawningHash.Add("FinishedSpawning", true);
        PhotonNetwork.LocalPlayer.SetCustomProperties(finishedSpawningHash);
    }
}
