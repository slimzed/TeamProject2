using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float enemyMovementSpeed = 2f;
    [Tooltip("-1 for left, 1 for right")]
    [SerializeField] private int moveDir = -1;

    private void Update()
    {
        transform.Translate(CalcMovement());
    }

    private Vector3 CalcMovement()
    {

        Vector3 movement = new Vector3(gameObject.transform.position.x + enemyMovementSpeed * moveDir * Time.deltaTime, gameObject.transform.position.y);

        return movement;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            moveDir = 0; // stops the object from moving when it collides with the player 
        }
    }


    private void DestroyObj()
    {
        Destroy(gameObject);
    }
}
