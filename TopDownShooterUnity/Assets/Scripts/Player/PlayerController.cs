using System.Collections;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    #region Script Description for inspector

    [Header("   SCRIPT DESCRIPTION:   ")]
    [Header("This script takes care of managing other \n" +
            "player \"stuff\"")]
    [Space(20)]

    [SerializeField] private bool iDoNothingLol;

    [Space(20)]

    #endregion

    private CinemachineVirtualCamera _virtualCamera;

    private PhotonView _view;
    private SpriteRenderer _spriteRenderer;

    [HideInInspector] public int playerIndex;
     public int health = 100;

    private void Start()
    {
        //Get Objects from scene
        _view = GetComponent<PhotonView>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _virtualCamera = GameObject.FindGameObjectWithTag("Virtual Camera").GetComponent<CinemachineVirtualCamera>();

        if (_view.IsMine)
            setUpVirtualCameraToFollowPlayer();
    }

    //This is done, so the PlayerSpawner can manage playerIndexes when players leave
    private void OnDestroy() => PhotonPlayerManager.Instance.indexOfMostRecentPlayerToLeave = playerIndex;

    private void setUpVirtualCameraToFollowPlayer() => _virtualCamera.Follow = transform;

}
