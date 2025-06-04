using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTest : MonoBehaviour
{
    private float timer;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] float fireDelay = 1f;
    [SerializeField] float bulletSpeed = 5f;
    void Start()
    {
        
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > fireDelay)
        {
            Rigidbody rig =  Instantiate(bulletPrefab, muzzlePoint.position, transform.rotation).GetComponent<Rigidbody>();
            rig.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
            timer = 0;
        }
    }
}
