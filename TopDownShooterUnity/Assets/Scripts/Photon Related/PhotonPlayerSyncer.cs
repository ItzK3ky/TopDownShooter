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

    [SerializeField] private bool iDoNothingLol;

    [Space(20)]

    #endregion
    
    private PhotonView photonView; //(Of this client)

    public List<GameObject> playerObjectsToSync = new List<GameObject>();
    public List<Vector2> playerObjectRigidbodyVelocity = new List<Vector2>();
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        PhotonNetwork.SendRate = 60;

        //Find Objects in scene
        photonView = GetComponent<PhotonView>();
	}

	private void FixedUpdate()
    {        
        #region Send my Rigidbody velocity to all other clients

        //Try statement because could all go wrong because there is no player yet since it waits 2 sec to spawn (SpawnPlayers script)
        try
        {
            GameObject playerOfThisClient = PhotonPlayerSpawner.Instance.playerObjectOfThisClient;
            Rigidbody2D rigidbodyOfPlayerOfThisClient = playerOfThisClient.GetComponent<Rigidbody2D>();
            PlayerController playerControllerOfPlayerOfThisClient = playerOfThisClient.GetComponent<PlayerController>();
        
            photonView.RPC("SetRigidBodyVelocityOnPlayerIndex", RpcTarget.All, rigidbodyOfPlayerOfThisClient.velocity, playerControllerOfPlayerOfThisClient.playerIndex);
        
            foreach (GameObject gameObjectInList in playerObjectsToSync)
            {
                StartCoroutine(UpdateTeleportingThreshold(gameObjectInList));
            }
        }
        catch
        {
        }

		#endregion
	}

	private IEnumerator UpdateTeleportingThreshold(GameObject gameObjectToUpdateTreshold)
    {
        PhotonTransformViewClassic photonTransformViewClassicOfGameObject = gameObjectToUpdateTreshold.GetComponent<PhotonTransformViewClassic>();

        PlayerController gameObjectPlayerController = gameObjectToUpdateTreshold.GetComponent<PlayerController>();
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
