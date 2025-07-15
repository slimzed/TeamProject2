using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    [SerializeField] private int highAttackDamage = 3;
    [SerializeField] private int medAttackDamage = 2;
    [SerializeField] private int lowAttackDamage = 1;
    
    
    
    private bool isEnemyCollided = false;
    private EnemyController currentEnemy;

    // debounce handling for inputs



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("move right");
        } else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("move left");
        }


        if (Input.GetKeyDown(KeyCode.Alpha1) && isEnemyCollided)
        {
            Debug.Log("attacked");
            currentEnemy.RemoveHealth(highAttackDamage);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("mid attack");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("low attack");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collided");
        if (collision.transform.CompareTag("Enemy"))
        {
            Debug.Log("enemy collision detected");
            isEnemyCollided = true;
            currentEnemy = collision.transform.GetComponent<EnemyController>();
        }
    }
}
