using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float enemyMovementSpeed = 2f;
    [Tooltip("-1 for left, 1 for right")]
    [SerializeField] public int moveDir = -1;
    [SerializeField] private int enemyHealth = 5;

    private void Update()
    {
        transform.Translate(CalcMovement());
    }

    private Vector3 CalcMovement()
    {
        float horizontalDisplacement = enemyMovementSpeed * moveDir * Time.deltaTime;
        return new Vector3(horizontalDisplacement, 0, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            moveDir = 0; // stops the object from moving when it collides with the player 
        }
    }
    public void RemoveHealth(int health)
    {
        enemyHealth-=health;
        Debug.Log(enemyHealth);
        if (enemyHealth <= 0)
        {
            Destroy(gameObject);

            // call the stat tracker singleton to add score.
        }
    }


    private void DestroyObj()
    {
        Destroy(gameObject);
    }
}
