using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private bool collided;
    private int damage = 20;
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("ayo cuh sup brah no cyap");
            PlayerController pd = collision.gameObject.GetComponent<PlayerController>();
            pd.TakeDamage(damage);
        }        
        if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag!= "Player"&&!collided)
        {
            collided = true;
            Destroy(gameObject);
        }
    }
}
