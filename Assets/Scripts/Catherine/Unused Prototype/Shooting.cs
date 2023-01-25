using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Sprite[] bulletSprite;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float bulletSpeed = 50f;

    private Vector3[] direction = {
    new Vector3(1, 0, 1),
    new Vector3(0, 0,1),
    new Vector3(-1,0, 1),
    new Vector3(-1,0, 0),
    new Vector3(-1, 0,-1),
    new Vector3(0, 0,-1),
    new Vector3(1, 0,-1),
    new Vector3(1,0, 0),
    };

    private float previousShot;

    private void Start()
    {
        previousShot = fireRate;
    }

    private void Update()
    {
        if (Input.GetButton("Gamepad_Fire1"))
        {
            if (Time.time > fireRate + previousShot)
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        GameObject go;

        go = Instantiate(bulletPrefab, transform.position, transform.rotation);
        go.GetComponent<Rigidbody>().velocity = direction[GetComponent<CharacterController>().lastDirection] * bulletSpeed;
        go.GetComponentInChildren<SpriteRenderer>().sprite = bulletSprite[GetComponent<CharacterController>().lastDirection];

        previousShot = Time.time;
    }
}
