using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PhotonPlayerSyncer : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;

    private List<GameObject> gameObjectsToSync = new List<GameObject>();
    private List<Vector2> gameObjectRigidbodyVelocity = new List<Vector2>();

    private void Start()
    {
        //Go through each gameObject and put Collider on isTrigger if it doesnt belong to this client
        foreach (GameObject gameObjectInList in gameObjectsToSync)
        {
            if (!gameObjectInList.GetComponent<PhotonView>().IsMine)
            {
                gameObjectInList.GetComponent<Collider2D>().isTrigger = true;
            }
        }
    }

    private void FixedUpdate()
    {
        //Set Rigidbody velocity of this client the same on all others too
        GameObject playerOfThisClient = null;
        Rigidbody2D rigidbodyOfPlayerOfThisClient = null;
        PlayerController playerControllerOfPlayerOfThisClient = null;

        //Try statement because could all go wrong because there is no player yet because it waits 2 sec to spawn (SpawnPlayers script)
        try
        {
            playerOfThisClient = findPlayerOfThisClient();
            rigidbodyOfPlayerOfThisClient = playerOfThisClient.GetComponent<Rigidbody2D>();
            playerControllerOfPlayerOfThisClient = playerOfThisClient.GetComponent<PlayerController>();
        
            photonView.RPC("setRigidBodyVelocityOnPlayerIndex", RpcTarget.All, rigidbodyOfPlayerOfThisClient.velocity, playerControllerOfPlayerOfThisClient.playerIndex);
        
            foreach (GameObject gameObjectInList in gameObjectsToSync)
            {
                increaseTeleportTresholdIfnecessary(gameObjectInList, getGameObjectSpeed(gameObjectInList));
            }
        }
        catch
        {
            Debug.Log("Couldnt make out player of this client");
        }
    }

    private float getGameObjectSpeed(GameObject gameObjectToCalculateSpeed)
    {
        PlayerController gameObjectPlayerController = gameObjectToCalculateSpeed.GetComponent<PlayerController>();

        float speed;
        Vector2 speedVector = gameObjectRigidbodyVelocity[gameObjectPlayerController.playerIndex];

        speed = Mathf.Max(speedVector.x, speedVector.y);
        if(speed == 0) speed = Mathf.Min(speedVector.x, speedVector.y);
        
        return speed;
    }

    private void increaseTeleportTresholdIfnecessary(GameObject gameObjectToIncreaseTreshold, float gameObjectSpeed)
    {
        PhotonTransformViewClassic photonTransformViewClassicOfGameObject = gameObjectToIncreaseTreshold.GetComponent<PhotonTransformViewClassic>();

        if (gameObjectSpeed != 0)
        {
            photonTransformViewClassicOfGameObject.m_PositionModel.TeleportIfDistanceGreaterThan = 3f;
        }
        else
        {
            photonTransformViewClassicOfGameObject.m_PositionModel.TeleportIfDistanceGreaterThan = 0.3f;
        }
    }

    private GameObject findPlayerOfThisClient()
    {
        foreach (GameObject gameObjectInList in gameObjectsToSync)
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
        gameObjectRigidbodyVelocity[playerIndexOfRigidbodyVelocity] = rigidbodyVelocity;
    }

    public void addGameObjectToSync(GameObject gameObjectToAdd)
    {
        gameObjectsToSync.Add(gameObjectToAdd);
        //Add GameObjects Rigidbody velocity to start with
        gameObjectRigidbodyVelocity.Add(gameObjectToAdd.GetComponent<Rigidbody2D>().velocity);
    }
}
