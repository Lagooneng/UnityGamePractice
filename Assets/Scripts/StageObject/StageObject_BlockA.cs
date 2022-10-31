using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObject_BlockA : MonoBehaviour
{
    bool destroyed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if( !destroyed && other.gameObject.tag != null )
        {
            if( other.gameObject.tag == "PlayerArm" ||
                other.gameObject.tag == "PlayerArmBullet" ||
                other.gameObject.tag == "EnemyArm" ||
                other.gameObject.tag == "EnemyArmBullet" )
            {
                destroyed = true;
                GetComponent<Animator>().enabled = true;
                GetComponent<Animator>().SetTrigger("Destroy");
                Destroy(gameObject, 0.5f);

                if( other.gameObject.tag == "EnemyArmBullet" )
                {
                    Destroy(other.gameObject);
                }
            }
        }
    }
}
