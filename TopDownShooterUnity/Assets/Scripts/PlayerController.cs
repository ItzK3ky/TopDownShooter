using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [Header("Player Attributes")]
    [SerializeField] private float playerSpeed;

    [Header("Objects")]
    [SerializeField] private Rigidbody2D rb;
    
    private Joystick joystick;

    private PhotonView photonView;

    public int playerIndex;


    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        //Find GameObjects in scene
        getJoystickObject();
    }

    private void FixedUpdate()
    {
        //Player Movement
        if (photonView.IsMine)
        {
            Vector2 movementVelocity;
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
                movementVelocity = joystick.Direction * playerSpeed * Time.deltaTime;
            else
                movementVelocity = new Vector2(Input.GetAxisRaw("Horizontal") * playerSpeed * Time.deltaTime, Input.GetAxisRaw("Vertical") * playerSpeed * Time.deltaTime);

            rb.velocity = movementVelocity;
        }
    }

    private void OnDestroy()
    {
        //This is done, so the PlayerSpawner can manage playerIndexes when players leave
        PlayerSpawner playerSpawner = GameObject.FindGameObjectWithTag("Player Spawner").GetComponent<PlayerSpawner>();
        playerSpawner.indexOfMostRecentPlayerToLeave = playerIndex;
    }

    //Find Joystick GameObject in Scene
    private void getJoystickObject()
    {
        joystick = Joystick.FindObjectOfType<Joystick>();
    }
}
