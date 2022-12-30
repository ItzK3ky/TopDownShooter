using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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

    public override void OnPlayerEnteredRoom(Player player) => StartCoroutine(HandleJoiningPlayer());

    public override void OnPlayerLeftRoom(Player Player) => StartCoroutine(HandleLeavingPlayer());

    /// <summary>
    /// Assigns playerIndexes to (other) joining players
    /// </summary>
    private IEnumerator HandleJoiningPlayer()
    {
        //Wait for RPC of joining client to send its "mostRecentPlayerToJoin" to this client
        //I do this, to get the Player GameObject, that just joined
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.2f);

            if (mostRecentPlayerToJoin != null)
                break;
        }
        //[mostRecentPlayerToJoin is set]

        //Set playerIndex for player that joined (locally)
        PlayerController playerControllerOfMostRecentPlayerToJoin = mostRecentPlayerToJoin.GetComponent<PlayerController>();
        playerControllerOfMostRecentPlayerToJoin.playerIndex = PhotonNetwork.PlayerList.Length - 1;

        PhotonPlayerSyncer.Instance.AddPlayerObjectToSync(mostRecentPlayerToJoin, mostRecentPlayerToJoin.GetComponent<PlayerController>().playerIndex);
        mostRecentPlayerToJoin.GetComponent<Collider2D>().isTrigger = true;

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

        //Removing leaving GameObject from list of Objects to be synced
        PhotonPlayerSyncer.Instance.RemovePlayerObjectToSync(indexOfMostRecentPlayerToLeave);

        //Going through each GameObject and downshifting each index if necessary
        foreach (GameObject playerObject in PhotonPlayerSyncer.Instance.playerObjectsToSync)
        {
            PlayerController playerControllerOfPlayerObjectInList = playerObject.GetComponent<PlayerController>();

            if (playerControllerOfPlayerObjectInList.playerIndex > indexOfMostRecentPlayerToLeave)
            {
                playerControllerOfPlayerObjectInList.playerIndex--;

                Debug.Log(playerObject + " now has the ID " + playerControllerOfPlayerObjectInList.playerIndex);
            }
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
            if (gameObject.GetPhotonView().IsMine)
                continue;

            PhotonPlayerSyncer.Instance.AddPlayerObjectToSync(gameObjectInArray, gameObjectInArray.GetComponent<PlayerController>().playerIndex);
            gameObjectInArray.GetComponent<Collider2D>().isTrigger = true;
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
