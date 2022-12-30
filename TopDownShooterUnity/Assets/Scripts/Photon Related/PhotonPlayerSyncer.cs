using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonPlayerSyncer : MonoBehaviour
{
    public static PhotonPlayerSyncer Instance { get; private set; }

    #region Script Description for inspector

    [Space]
    [Header("   SCRIPT DESCRIPTION:   ")]
    [Header("This script adjustes the teleporting \n" +
            "treshold for the players, to make \n" +
            "movement synchronize on all clients as \n" +
            "smoothly as possible.")]
    [Space(20)]

    public bool iDoNothingLol;

    [Space(20)]

    #endregion
    
    private PhotonView photonView; //(Of this client)

    [HideInInspector] public List<GameObject> playerObjectsToSync = new List<GameObject>();
    [HideInInspector] public List<Vector2> playerObjectRigidbodyVelocity = new List<Vector2>();
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        //Find Objects in scene
        photonView = GetComponent<PhotonView>();

        #region Put each Collider on isTrigger if it doesnt belong to this client
        foreach (GameObject gameObjectInList in playerObjectsToSync)
        {
            if (!gameObjectInList.GetComponent<PhotonView>().IsMine)
            {
                gameObjectInList.GetComponent<Collider2D>().isTrigger = true;
            }
        }
		#endregion
	}

	private void Update()
    {
        #region Send my Rigidbody velocity to all other clients

		GameObject playerOfThisClient;
        Rigidbody2D rigidbodyOfPlayerOfThisClient;
        PlayerController playerControllerOfPlayerOfThisClient;

        //Try statement because could all go wrong because there is no player yet since it waits 2 sec to spawn (SpawnPlayers script)
        try
        {
            playerOfThisClient = FindPlayerObjectOfThisClient();
            rigidbodyOfPlayerOfThisClient = playerOfThisClient.GetComponent<Rigidbody2D>();
            playerControllerOfPlayerOfThisClient = playerOfThisClient.GetComponent<PlayerController>();
        
            photonView.RPC("SetRigidBodyVelocityOnPlayerIndex", RpcTarget.All, rigidbodyOfPlayerOfThisClient.velocity, playerControllerOfPlayerOfThisClient.playerIndex);
        
            foreach (GameObject gameObjectInList in playerObjectsToSync)
            {
                StartCoroutine(increaseTeleportTresholdIfnecessary(gameObjectInList));
            }
        }
        catch
        {
        }

		#endregion
	}

	private IEnumerator increaseTeleportTresholdIfnecessary(GameObject gameObjectToIncreaseTreshold)
    {
        PhotonTransformViewClassic photonTransformViewClassicOfGameObject = gameObjectToIncreaseTreshold.GetComponent<PhotonTransformViewClassic>();
        PlayerController gameObjectPlayerController = gameObjectToIncreaseTreshold.GetComponent<PlayerController>();

        Vector2 speedVector = playerObjectRigidbodyVelocity[gameObjectPlayerController.playerIndex];
        
        if (speedVector != Vector2.zero) //If moving
        {
            photonTransformViewClassicOfGameObject.m_PositionModel.TeleportIfDistanceGreaterThan = 3f;
        }
        else //If not moving
        {
            //Wait 0.3 seconds to put it back to 0.3 after moving, to smooth out movement
            if(photonTransformViewClassicOfGameObject.m_PositionModel.TeleportIfDistanceGreaterThan == 3f)
                yield return new WaitForSeconds(0.3f);

            photonTransformViewClassicOfGameObject.m_PositionModel.TeleportIfDistanceGreaterThan = 0.1f;
        }
    }

    /// <summary>
    /// This method goes through all Player GameObjects 
    /// and returns the one, that belongs to this client
    /// </summary>
    private GameObject FindPlayerObjectOfThisClient()
    {
        foreach (GameObject gameObjectInList in playerObjectsToSync)
        {
            PhotonView photonViewOfGameObjectInList = gameObjectInList.GetComponent<PhotonView>();
            if (photonViewOfGameObjectInList.IsMine)
            {
                return gameObjectInList;
            }
        }

        return null;
    }

    [PunRPC]
    private void SetRigidBodyVelocityOnPlayerIndex(Vector2 rigidbodyVelocity, int playerIndexOfRigidbodyVelocity)
    {
        playerObjectRigidbodyVelocity[playerIndexOfRigidbodyVelocity] = rigidbodyVelocity;
    }

    public void AddPlayerObjectToSync(GameObject playerObjectToAdd, int playerIndex)
    {
        playerObjectsToSync.Insert(playerIndex, playerObjectToAdd);
        playerObjectRigidbodyVelocity.Add(playerObjectToAdd.GetComponent<Rigidbody2D>().velocity);
    }

    public void RemovePlayerObjectToSync(int playerIndex)
    {
        playerObjectsToSync.RemoveAt(playerIndex);
        playerObjectRigidbodyVelocity.RemoveAt(playerIndex);
    }
}