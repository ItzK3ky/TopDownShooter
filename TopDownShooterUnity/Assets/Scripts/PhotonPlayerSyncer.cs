using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PhotonPlayerSyncer : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;

    public List<GameObject> playerObjectsToSync = new List<GameObject>();
    public List<Vector2> playerObjectRigidbodyVelocity = new List<Vector2>();

    private void Start()
    {
        //Go through each gameObject and put Collider on isTrigger if it doesnt belong to this client
        foreach (GameObject gameObjectInList in playerObjectsToSync)
        {
            if (!gameObjectInList.GetComponent<PhotonView>().IsMine)
            {
                gameObjectInList.GetComponent<Collider2D>().isTrigger = true;
            }
        }
    }

    private void FixedUpdate()
    {
        #region Send Rigidbody velocity of this client to all other clients

		GameObject playerOfThisClient;
        Rigidbody2D rigidbodyOfPlayerOfThisClient;
        PlayerController playerControllerOfPlayerOfThisClient;

        //Try statement because could all go wrong because there is no player yet since it waits 2 sec to spawn (SpawnPlayers script)
        try
        {
            playerOfThisClient = findPlayerObjectOfThisClient();
            rigidbodyOfPlayerOfThisClient = playerOfThisClient.GetComponent<Rigidbody2D>();
            playerControllerOfPlayerOfThisClient = playerOfThisClient.GetComponent<PlayerController>();
        
            photonView.RPC("setRigidBodyVelocityOnPlayerIndex", RpcTarget.All, rigidbodyOfPlayerOfThisClient.velocity, playerControllerOfPlayerOfThisClient.playerIndex);
        
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

            photonTransformViewClassicOfGameObject.m_PositionModel.TeleportIfDistanceGreaterThan = 0.3f;
        }
    }

    private GameObject findPlayerObjectOfThisClient()
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
    private void setRigidBodyVelocityOnPlayerIndex(Vector2 rigidbodyVelocity, int playerIndexOfRigidbodyVelocity)
    {
        playerObjectRigidbodyVelocity[playerIndexOfRigidbodyVelocity] = rigidbodyVelocity;
    }

    public void addPlayerObjectToSync(GameObject playerObjectToAdd, int playerIndex)
    {
        playerObjectsToSync.Insert(playerIndex, playerObjectToAdd);
        playerObjectRigidbodyVelocity.Add(playerObjectToAdd.GetComponent<Rigidbody2D>().velocity);
    }

    public void removePlayerObjectToSync(int playerIndex)
    {
        playerObjectsToSync.RemoveAt(playerIndex);
        playerObjectRigidbodyVelocity.RemoveAt(playerIndex);
    }
}
