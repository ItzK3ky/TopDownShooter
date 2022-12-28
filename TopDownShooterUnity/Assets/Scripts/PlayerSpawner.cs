using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;

    private CinemachineVirtualCamera virtualCamera;
    private PhotonPlayerSyncer photonPlayerSyncer;

    private PhotonView photonView; //(Of this GameObject)

    private GameObject spawnedPlayer; //Player, that this script spawns (also player of this client)

    private GameObject mostRecentPlayerToJoin;
    public GameObject mostRecentPlayerToLeave;


    void Start()
    {
        //Get Objects From Scene
        virtualCamera = GameObject.FindGameObjectWithTag("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        photonPlayerSyncer = GameObject.FindGameObjectWithTag("Photon Player Syncer").GetComponent<PhotonPlayerSyncer>();
        photonView = GetComponent<PhotonView>();
        
        Debug.Log("I joined the room");
        StartCoroutine(spawnPlayer());
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        Debug.Log("Someone joined the room");
        StartCoroutine(handleJoiningPlayer());
    }
    
    public override void OnPlayerLeftRoom(Player Player)
    {
        Debug.Log("Someone left the room");
        StartCoroutine(handleLeavingPlayer());
    }

    /// <summary>
    /// Spawns player on on Photon Room (so it's visible for all players)
    /// and  assigns an playerIndex to the player
    /// </summary>
    private IEnumerator spawnPlayer()
    {
        handlePlayersAlreadyInRoom();

        yield return new WaitForSecondsRealtime(2f);

        spawnedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
        spawnedPlayer.GetComponent<PlayerController>().playerIndex = PhotonNetwork.PlayerList.Length - 1;

        //Send other clients, already in room, this clients gameobject
        photonView.RPC("setMostRecentPlayerToJoin", RpcTarget.Others, spawnedPlayer.GetPhotonView().ViewID);
        
        setupVirtualCameraToFollowPlayer();
        addPlayerToBeSynced();
    }

    /// <summary>
    /// Assigns playerIndexes to (other) joining players
    /// </summary>
    private IEnumerator handleJoiningPlayer()
    {
        Debug.Log("Handling joining player...");

        //Wait for RPC of joining client to send its "mostRecentPlayerToJoin" to this client
        //I do this, to get the Player GameObject, that just joined
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.2f);

            if (mostRecentPlayerToJoin != null)
                break;
        }

        PlayerController playerControllerOfMostRecentPlayerToJoin = mostRecentPlayerToJoin.GetComponent<PlayerController>();
        playerControllerOfMostRecentPlayerToJoin.playerIndex = PhotonNetwork.PlayerList.Length - 1;

        photonPlayerSyncer.addPlayerObjectToSync(mostRecentPlayerToJoin, mostRecentPlayerToJoin.GetComponent<PlayerController>().playerIndex);

        //Set collider of other players to isTrigger
        mostRecentPlayerToJoin.GetComponent<Collider2D>().isTrigger = true;

        //Reset mostRecentPlayerToJoin for the next time
        mostRecentPlayerToJoin = null;

        Debug.Log("Handled joining Player!. Joined player " + mostRecentPlayerToJoin + " has the ID " + playerControllerOfMostRecentPlayerToJoin.playerIndex);
    }

    /// <summary>
    /// Shifts down all playerIndexes, if a player leaves
    /// </summary>
    private IEnumerator handleLeavingPlayer()
    {
        Debug.Log("Handling leaving player...");

        //Wait for mostRecentPlayer to be set
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            if (mostRecentPlayerToLeave != null)
                break;
        }

        //Removing GameObject from list of Objects to be removed
        photonPlayerSyncer.removePlayerObjectToSync(mostRecentPlayerToLeave.GetComponent<PlayerController>().playerIndex);

        //Going through each GameObject and downshifting each index if necessary
        foreach (GameObject playerObject in photonPlayerSyncer.playerObjectsToSync)
        {
            PlayerController playerControllerOfPlayerThatLeft = mostRecentPlayerToLeave.GetComponent<PlayerController>();
            PlayerController playerControllerOfPlayerObjectInList = playerObject.GetComponent<PlayerController>();
            if (playerControllerOfPlayerObjectInList.playerIndex > playerControllerOfPlayerThatLeft.playerIndex)
            {
                //Remove player that left from PhotonPlayerSyncer
                photonPlayerSyncer.playerObjectsToSync.RemoveAt(playerControllerOfPlayerThatLeft.playerIndex);
                photonPlayerSyncer.playerObjectRigidbodyVelocity.RemoveAt(playerControllerOfPlayerThatLeft.playerIndex);

                playerControllerOfPlayerObjectInList.playerIndex--;

                Debug.Log(playerObject + " now has the ID " + playerControllerOfPlayerObjectInList.playerIndex);
            }
        }

        //Reset mostRecentPlayerToLeave for the next time
        mostRecentPlayerToLeave = null;

        Debug.Log("Handled leaving player!");
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
    
    /// <summary>
    /// Adds all players, that are already in the room to the list
    /// of GameObjects to be synced, and puts their colliders on isTrigger
    /// </summary>
    private void handlePlayersAlreadyInRoom()
    {
		GameObject[] arrayOfPlayerGameObjectsInScene = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject gameObjectInArray in arrayOfPlayerGameObjectsInScene)
        {
            //"Only handle other players, not this client"
            if (gameObject.GetPhotonView().IsMine)
                continue;

            photonPlayerSyncer.addPlayerObjectToSync(gameObjectInArray, gameObjectInArray.GetComponent<PlayerController>().playerIndex);
            gameObjectInArray.GetComponent<Collider2D>().isTrigger = true;
        }
	}



	private void setupVirtualCameraToFollowPlayer()
    {
        virtualCamera.Follow = spawnedPlayer.transform;
    }

    private void addPlayerToBeSynced()
    {
        photonPlayerSyncer.addPlayerObjectToSync(spawnedPlayer, spawnedPlayer.GetComponent<PlayerController>().playerIndex);
    }
}
