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
    
    private PhotonView _view; //(Of this client)

    public List<GameObject> playerObjectsToSync = new List<GameObject>();
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        //Find Objects in scene
        _view = GetComponent<PhotonView>();
    }

	private void UpdateTeleportingThreshold(GameObject gameObjectToUpdateTreshold)
    {
        PhotonTransformViewClassic photonTransformViewClassicOfGameObject = gameObjectToUpdateTreshold.GetComponent<PhotonTransformViewClassic>();

        PlayerController gameObjectPlayerController = gameObjectToUpdateTreshold.GetComponent<PlayerController>();
        Vector2 speedVector = gameObjectToUpdateTreshold.GetComponent<Rigidbody2D>().velocity;

        if (speedVector != Vector2.zero) //If moving
        {
            photonTransformViewClassicOfGameObject.m_PositionModel.TeleportIfDistanceGreaterThan = 3f;
        }
        else //If not moving
        {
            photonTransformViewClassicOfGameObject.m_PositionModel.TeleportIfDistanceGreaterThan = 0.01f;
        }
    }

    public void AddPlayerObjectToSync(GameObject playerObjectToAdd, int playerIndex)
    {
        playerObjectsToSync.Insert(playerIndex, playerObjectToAdd);
    }

    public void RemovePlayerObjectToSync(int playerIndex)
    {
        playerObjectsToSync.RemoveAt(playerIndex);
    }
}
