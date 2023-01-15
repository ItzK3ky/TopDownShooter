using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// This Script is only ever meant to be sitting on the Player Manager Object
/// </summary>
public class PhotonPlayerManager : MonoBehaviourPunCallbacks
{
    public static PhotonPlayerManager Instance { get; private set; }

	#region Script Description for inspector

    [Header("   SCRIPT DESCRIPTION:   ")]
    [Header("This script handles all other players \n" +
            "joining and/or leaving")]
    [Space(20)]

    [SerializeField] private bool iDoNothingLol;

    [Space(20)]

	#endregion

    private GameObject mostRecentPlayerToJoin;
    [HideInInspector] public int indexOfMostRecentPlayerToLeave = -1; //Is by default -1, because no player is ever gonna have index -1

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
    }

    void Start() => StartCoroutine(handlePlayersAlreadyInRoom());

    public override void OnPlayerEnteredRoom(Player player) => StartCoroutine(HandleJoiningPlayer(player));

    public override void OnPlayerLeftRoom(Player Player) => StartCoroutine(HandleLeavingPlayer());

    /// <summary>
    /// Assigns playerIndexes to (other) joining players
    /// </summary>
    private IEnumerator HandleJoiningPlayer(Player player)
    {
        //Wait for RPC of joining client to send its "mostRecentPlayerToJoin" to this client
        //I do this, to get the Player GameObject, that just joined
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            bool finishedSpawning = false;
            try { finishedSpawning = (bool)player.CustomProperties["FinishedSpawning"]; }
            catch { }

            if (mostRecentPlayerToJoin != null && finishedSpawning == true)
                break;
        }
        //[mostRecentPlayerToJoin is set]

        //Set playerIndex for player that joined (locally)
        PlayerController playerControllerOfMostRecentPlayerToJoin = mostRecentPlayerToJoin.GetComponent<PlayerController>();
        playerControllerOfMostRecentPlayerToJoin.playerIndex = PhotonNetwork.PlayerList.Length - 1;

        //Change Nickname text to nickname
        mostRecentPlayerToJoin.GetComponentInChildren<TMP_Text>().text = mostRecentPlayerToJoin.GetPhotonView().Owner.NickName;

        //Change Player color
        byte playerColorR = (byte)player.CustomProperties["PlayerColorR"];
        byte playerColorG = (byte)player.CustomProperties["PlayerColorG"];
        byte playerColorB = (byte)player.CustomProperties["PlayerColorB"];

        Color32 playerColor = new Color32(playerColorR, playerColorG, playerColorB, 255);
        mostRecentPlayerToJoin.GetComponent<SpriteRenderer>().color = playerColor;

        //Reset mostRecentPlayerToJoin for the next time
        mostRecentPlayerToJoin = null;
    }

    /// <summary>
    /// Shifts down all playerIndexes, when a player leaves, and removes the player
    /// that left from the list of Objects to be synced (PhotonPlayerSyncer.cs)
    /// </summary>
    private IEnumerator HandleLeavingPlayer()
    {
        //Wait for mostRecentPlayerToLeave to be set
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            if (indexOfMostRecentPlayerToLeave != -1)
                break;
        }
        //[mostRecentPlayerToLeave is set]

        //Going through each GameObject and downshifting each index if 
        GameObject[] playerGameObjectsInScene = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObject in playerGameObjectsInScene)
        {
            PlayerController playerControllerOfPlayerObjectInList = playerObject.GetComponent<PlayerController>();

            if (playerControllerOfPlayerObjectInList.playerIndex > indexOfMostRecentPlayerToLeave)
                playerControllerOfPlayerObjectInList.playerIndex--;
        }

        //Reset indexOfMostRecentPlayerToLeave for the next time
        indexOfMostRecentPlayerToLeave = -1;
    }

    /// <summary>
    /// Adds all players, that are already in the room to the list
    /// of GameObjects to be synced, and puts their colliders on isTrigger
    /// </summary>
    private IEnumerator handlePlayersAlreadyInRoom()
    {
        //Wait for game to spawn in other player GameObjects
        yield return new WaitForSecondsRealtime(0.2f);

        GameObject[] arrayOfPlayerGameObjectsInScene = GameObject.FindGameObjectsWithTag("Player");

        //Handle each player GameObject already in room
        foreach (GameObject gameObjectInArray in arrayOfPlayerGameObjectsInScene)
        {
            //"Only handle other players, not this client"
            if (gameObjectInArray.GetPhotonView().IsMine)
                continue;

            //Change Nickname text to Nickname
            gameObjectInArray.GetComponentInChildren<TMP_Text>().text = gameObjectInArray.GetPhotonView().Owner.NickName;

            //Change Player 
            byte playerColorR = (byte)gameObjectInArray.GetPhotonView().Owner.CustomProperties["PlayerColorR"];
            byte playerColorG = (byte)gameObjectInArray.GetPhotonView().Owner.CustomProperties["PlayerColorG"];
            byte playerColorB = (byte)gameObjectInArray.GetPhotonView().Owner.CustomProperties["PlayerColorB"];

            Color32 playerColor = new Color32(playerColorR, playerColorG, playerColorB, 255);
            gameObjectInArray.GetComponent<SpriteRenderer>().color = playerColor;
        }
	}

    /// <summary>
    /// Sets the "mostRecentPlayerToJoin" variable to the player, that last joined the room.
    /// Takes in "photonID" to work around not being able to send GameObjects through RPCs
    /// </summary>
    [PunRPC]
    private void setMostRecentPlayerToJoin(int photonID)
    {
        GameObject playerObject = PhotonNetwork.GetPhotonView(photonID).gameObject;
        mostRecentPlayerToJoin = playerObject;
    }
}
