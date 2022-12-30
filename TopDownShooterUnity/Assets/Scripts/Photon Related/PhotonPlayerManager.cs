using System.Collections;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;

/// <summary>
/// This Script is only ever meant to be sitting on the Player Manager Object
/// </summary>
public class PhotonPlayerManager : MonoBehaviourPunCallbacks
{
    public static PhotonPlayerManager Instance { get; private set; }

	#region Script Description for inspector

	[Space]
    [Header("   SCRIPT DESCRIPTION:   ")]
    [Header("This script takes care of spawning the \n" +
            "player in (on the \"Photon network\") and\n" +
            "and also takes care of other players \n" +
            "joining.")]
    [Space(20)]

    public bool iDoNothingLol;

    [Space(20)]

	#endregion

	[Header("   SERIALIZEFIELDS:")]
    [Space(10)]

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private PhotonView photonView; //(Of this GameObject)

    private GameObject playerOfThisClient; //Player, that this script spawns (also player of this client)

    private GameObject mostRecentPlayerToJoin;
    [HideInInspector] public int indexOfMostRecentPlayerToLeave = -1; //Is by default -1, because no player is ever gonna have index -1

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        //Get Objects From Scene
        photonView = GetComponent<PhotonView>();

        EditorGUILayout.HelpBox("Ball", MessageType.Info);

            Debug.Log("I joined the room");
        StartCoroutine(SpawnPlayer());
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        Debug.Log("Someone joined the room");
        StartCoroutine(HandleJoiningPlayer());
    }
    
    public override void OnPlayerLeftRoom(Player Player)
    {
        Debug.Log("Someone left the room");
        StartCoroutine(HandleLeavingPlayer());
    }

    /// <summary>
    /// Spawns player on on Photon Room (so it's visible for all players)
    /// and  assigns an playerIndex to the player
    /// </summary>
    private IEnumerator SpawnPlayer()
    {
        StartCoroutine(handlePlayersAlreadyInRoom());

        yield return new WaitForSecondsRealtime(2f);

        playerOfThisClient = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
        playerOfThisClient.GetComponent<PlayerController>().playerIndex = PhotonNetwork.PlayerList.Length - 1;

        //Send other clients, already in room, this clients gameobject
        photonView.RPC("setMostRecentPlayerToJoin", RpcTarget.Others, playerOfThisClient.GetPhotonView().ViewID);

        //Add "myself" to be synced
        PhotonPlayerSyncer.Instance.AddPlayerObjectToSync(playerOfThisClient, playerOfThisClient.GetComponent<PlayerController>().playerIndex);

        setupVirtualCameraToFollowPlayer();
    }

    /// <summary>
    /// Assigns playerIndexes to (other) joining players
    /// </summary>
    private IEnumerator HandleJoiningPlayer()
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

        PhotonPlayerSyncer.Instance.AddPlayerObjectToSync(mostRecentPlayerToJoin, mostRecentPlayerToJoin.GetComponent<PlayerController>().playerIndex);

        //Set collider of other players to isTrigger
        mostRecentPlayerToJoin.GetComponent<Collider2D>().isTrigger = true;

        //Reset mostRecentPlayerToJoin for the next time
        mostRecentPlayerToJoin = null;

        Debug.Log("Handled joining Player!. Joined player " + mostRecentPlayerToJoin + " has the ID " + playerControllerOfMostRecentPlayerToJoin.playerIndex);
    }

    /// <summary>
    /// Shifts down all playerIndexes, when a player leaves, and removes the player
    /// that left from the list of Objects to be synced (PhotonPlayerSyncer.cs)
    /// </summary>
    private IEnumerator HandleLeavingPlayer()
    {
        Debug.Log("Handling leaving player...");

        //Wait for mostRecentPlayer to be set
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            if (indexOfMostRecentPlayerToLeave != -1)
                break;
        }

        //Removing GameObject from list of Objects to be synced
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

        Debug.Log("Handled leaving player!");
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
    
	private void setupVirtualCameraToFollowPlayer()
    {
        virtualCamera.Follow = playerOfThisClient.transform;
    }
}
