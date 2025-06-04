using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTest : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" )
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.TakeDamage(1, transform);
            Destroy(gameObject);
        }
    }
}
