using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Transform player;
    [SerializeField] private Sprite noShield;
    [SerializeField] private Sprite shield;
    [SerializeField] private float moveSpeed;
    public bool isShield;

    public bool isShieldAlive = true;

    //private int maxDist = 20;
    private int minDist = 1;

    private Vector3 dir;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (GameObject.Find("Prototype_Shield") != null)
        {
            isShieldAlive = true;
        }
        else { isShieldAlive = false; }

        if (isShield)
        {
            transform.Translate(dir * moveSpeed * Time.deltaTime);

            if (transform.position.z <= -10)
            {
                dir = Vector3.forward;
            }
            else if (transform.position.z >= 10)
            {
                dir = Vector3.back;
            }
        }
        else
        {
            if(player != null)
            {
                transform.LookAt(player);

                if (Vector3.Distance(transform.position, player.position) >= minDist)
                {
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                }
            }
        }

        if (!isShield)
        {
            if(isShieldAlive)
            {
                GetComponentInChildren<SpriteRenderer>().sprite = shield;
            }
            else
            {
                GetComponentInChildren<SpriteRenderer>().sprite = noShield;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<CharacterController>())
        {
            Camera.main.gameObject.transform.parent.parent = null;
            Destroy(collision.gameObject);
        }
    }
}
