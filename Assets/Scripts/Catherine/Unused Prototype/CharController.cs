using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private GameObject nextRoom, winScreen;
    [SerializeField] private Material charTexture, dashTexture;

    [SerializeField] private string[] idleDirections = { "Idle N", "Idle NW", "Idle W", "Idle SW", "Idle S", "Idle SE", "Idle E", "IdleNE"};
    [SerializeField] private string[] runDirections = { "Run N", "Run NW", "Run W", "Run SW", "Run S", "Run SE", "Run E", "Run NE" };
    private string[] directionArray;
    public int lastDirection;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float startDashTime;

    private float dashTime;
    private float initialMoveSpeed;
    private Vector3 direction;
    private Vector3 forward, right;

    private bool isDashing = false;

    private void Awake()
    {
        //anim = GetComponent<Animator>();
    }
    void Start()
    {
        forward = Camera.main.transform.forward; // Set forward to equal the camera's forward vector
        forward.y = 0; // make sure y is 0
        forward = Vector3.Normalize(forward); // make sure the length of vector is set to a max of 1.0
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward; // set the right-facing vector to be facing right relative to the camera's forward vector

        dashTime = startDashTime;
        initialMoveSpeed = moveSpeed;
    }

    void Update()
    {
        if(GameObject.FindGameObjectWithTag("Enemy") == null)
        {
            if(gameObject.transform.parent.name == "Game")
            {
                transform.parent.gameObject.SetActive(false);
                if(nextRoom != null)
                nextRoom.SetActive(true);
            }
            else
            {
                if(winScreen != null)
                winScreen.SetActive(true);
            }
        }

        if(isDashing)
        {
            GetComponentInChildren<MeshRenderer>().material = dashTexture;
            dashTime -= Time.deltaTime;
        }
        if( dashTime <= 0)
        {
            GetComponentInChildren<MeshRenderer>().material = charTexture;
            dashTime = startDashTime;
            moveSpeed = initialMoveSpeed;
            isDashing = false;
        }    
        
        if (Input.GetButtonDown("Gamepad_Dash"))
        {
            isDashing = true;
            moveSpeed += dashSpeed;
        }

        Move();
    }

    void Move()
    {
        direction = new Vector2(Input.GetAxis("Gamepad_Horizontal"), Input.GetAxis("Gamepad_Vertical"));
        SetDirection(direction);
        Vector3 rightMovement = right * moveSpeed * Time.deltaTime * Input.GetAxis("Gamepad_Horizontal");
        Vector3 upMovement = forward * moveSpeed * Time.deltaTime * Input.GetAxis("Gamepad_Vertical");

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
    }

    public void SetDirection(Vector2 _direction)
    {
        // player not moving
        if (_direction.magnitude < 0.01f)
        {
            directionArray = idleDirections;
        }
        else
        {
            directionArray = runDirections;
            lastDirection = DirectionToIndex(_direction);
        }

        //anim.Play(directionArray[lastDirection]);
    }

    private int DirectionToIndex(Vector2 _direction)
    {
        Vector2 norDirection = _direction.normalized;

        float axis = 360 / 8;
        float angle = Vector2.SignedAngle(Vector2.up, norDirection);

        if (angle < 0)
        {
            angle += 360;
        }

        return Mathf.FloorToInt(angle / axis);
    }
}
