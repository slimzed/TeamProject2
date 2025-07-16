using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Weak,
    Normal,
    Elite
}


public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType;



    
    
    [SerializeField] private float enemyMovementSpeed = 2f;
    [Tooltip("-1 for left, 1 for right")]
    [SerializeField] public int moveDir = -1;
    [SerializeField] private int enemyHealth = 5;


    // change from enemy to enemy
    private KeyCode[] requiredSequence; 


    public KeyCode[] ComboSequence => requiredSequence; // creates a public variable that just inherits from the requiredSequence private variable

    private void Awake()
    {
        InitializeEnemyCombos(enemyType); // enemy type is set within the editor

    }

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
    private void InitializeEnemyCombos(EnemyType enemyType)
    {
        switch (enemyType) 
        {
            case EnemyType.Weak:
                requiredSequence = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2 };
                break;
            case EnemyType.Normal:
                requiredSequence = new KeyCode[] { KeyCode.Alpha2, KeyCode.Alpha3 };
                break;
            case EnemyType.Elite:
                requiredSequence = new KeyCode[] { KeyCode.Alpha3, KeyCode.Alpha1, KeyCode.Alpha2 };
                break;
        }
    }
    public void KillEnemy()
    {
        Destroy(gameObject);
    }




}
