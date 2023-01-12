using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

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

    public Color32 redColor;
    public Color32 greenColor;
    public Color32 cyanColor;
    public Color32 yellowColor;
    public Color32 grayColor;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        redColor = new Color32(255, 0, 0, 255);
        greenColor = new Color32(0, 255, 0, 255);
        cyanColor = new Color32(0, 255, 255, 255);
        yellowColor = new Color32(255, 255, 0, 255);
        grayColor = new Color32(150, 150, 150, 255);
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

        //Send this clients player object to all other clients
        PhotonPlayerManager.Instance.photonView.RPC("setMostRecentPlayerToJoin", RpcTarget.Others, playerObjectOfThisClient.GetPhotonView().ViewID);

        //Change Nickname text to Nickname
        playerObjectOfThisClient.GetComponentInChildren<TMP_Text>().text = playerObjectOfThisClient.GetPhotonView().Owner.NickName;

        //Change Player Color
        playerObjectOfThisClient.GetComponent<SpriteRenderer>().color = GetRandomPlayerColor();
    }

    private Color32 GetRandomPlayerColor()
    {
        switch(Random.Range(0, 4))
        {
            case 0:
                return redColor;
            case 1:
                return greenColor;
            case 2:
                return cyanColor;
            case 3:
                return yellowColor;
            case 4:
                return grayColor;
        }

        return new Color32(0, 0, 0, 0);
    }
}
