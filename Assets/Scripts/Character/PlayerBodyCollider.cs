using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyCollider : MonoBehaviour
{
    PlayerController playerCtrl;

    private void Awake()
    {
        playerCtrl = transform.parent.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("Player OnTriggerEnter2D : " + other.name);

        if( other.tag == "EnemyArm" )
        {
            EnemyController enemyCtrl = other.GetComponentInParent<EnemyController>();

            if( enemyCtrl.attackEnabled )
            {
                enemyCtrl.attackEnabled = false;
                playerCtrl.dir = (playerCtrl.transform.position.x < enemyCtrl.transform.position.x) ? +1 : -1;
                playerCtrl.AddForceAnimatorVx(-enemyCtrl.attackNockBackVector.x);
                playerCtrl.AddForceAnimatorVy(enemyCtrl.attackNockBackVector.y);
                playerCtrl.ActionDamage(enemyCtrl.attackDamage);
            }
        }
        else if( other.tag == "EnemyArmBullet" )
        {
            FireBullet fireBullet = other.transform.GetComponent<FireBullet>();

            if( fireBullet.attackEnabled )
            {
                fireBullet.attackEnabled = false;

                playerCtrl.dir = (playerCtrl.transform.position.x < fireBullet.transform.position.x) ? +1 : -1;
                playerCtrl.AddForceAnimatorVx(-fireBullet.attackNockBackVector.x);
                playerCtrl.AddForceAnimatorVy(fireBullet.attackNockBackVector.y);
                playerCtrl.ActionDamage(fireBullet.attackDamage);
                Destroy(other.gameObject);
            }
        }
        else if( other.tag == "CameraTrigger" )
        {
            Camera.main.GetComponent<CameraFollow>().SetCamera(other.GetComponent<StageTrigger_Camera>().param);
        }
        else if( other.tag == "EventTrigger" )
        {
            other.SendMessage("OnTriggerEnter2D_PlayerEvent", gameObject);
        }
        else if( other.name == "DeathCollider" )
        {
            playerCtrl.Dead(true);
        }
        else if( other.name == "DeathCollider_Rock" )
        {
            if( playerCtrl.transform.position.y < other.transform.position.y )
            {
                if( ( playerCtrl.transform.position.x < other.transform.position.x &&
                    other.transform.parent.GetComponent<Rigidbody2D>().velocity.x < -1.0f ) ||
                    ( playerCtrl.transform.position.x > other.transform.position.x &&
                    other.transform.parent.GetComponent<Rigidbody2D>().velocity.x > +1.0f ) ||
                    ( other.transform.parent.GetComponent<Rigidbody2D>().velocity.y < -1.0f ) )
                {
                    playerCtrl.Dead(true);
                } 
            }
        }
        else if( other.tag == "DestroySwitch" )
        {
            other.GetComponent<StageObject_DestroySwitch>().DestroyStageObject();
        }
        else if( other.tag == "Item" )
        {
            if( other.name == "Item_Koban" )
            {
                PlayerController.score += 10;
            }
            else if( other.name == "Item_Ohoban" )
            {
                PlayerController.score += 100000;
            }
            else if( other.name == "Item_Hyoutan" )
            {
                playerCtrl.SetHp(playerCtrl.hp + playerCtrl.hpMax / 3, playerCtrl.hpMax);
            }
            else if( other.name == "Item_Makimono" )
            {
                playerCtrl.superMode = true;
                playerCtrl.GetComponent<Stage_AfterImage>().afterImageEnabled = true;
                playerCtrl.basScaleX = 2.0f;
                playerCtrl.transform.localScale = new Vector3(playerCtrl.basScaleX, 2.0f, 1.0f);
                Invoke("SuperModeEnd", 10.0f);
            }
            else if( other.name == "Item_Key_A" )
            {
                PlayerController.score += 10000;
                PlayerController.itemKeyA = true;
                GameObject.Find("HUD_Item_Key_A").GetComponent<SpriteRenderer>().enabled = true;
            }
            else if (other.name == "Item_Key_B")
            {
                PlayerController.score += 10000;
                PlayerController.itemKeyB = true;
                GameObject.Find("HUD_Item_Key_B").GetComponent<SpriteRenderer>().enabled = true;
            }
            else if (other.name == "Item_Key_C")
            {
                PlayerController.score += 10000;
                PlayerController.itemKeyC = true;
                GameObject.Find("HUD_Item_Key_C").GetComponent<SpriteRenderer>().enabled = true;
            }


            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if( other.tag == "DamageObject" )
        {
            float damage = other.GetComponent<StageObject_Damage>().damage * Time.fixedDeltaTime;

            if( playerCtrl.SetHp(playerCtrl.hp - damage, playerCtrl.hpMax) )
            {
                playerCtrl.hp = 0;
                playerCtrl.Dead(true);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if( collision.gameObject.name == "DeathCollider" )
        {
            playerCtrl.Dead(true);
        }
    }

    void SuperModeEnd()
    {
        playerCtrl.superMode = false;
        playerCtrl.GetComponent<Stage_AfterImage>().afterImageEnabled = false;
        playerCtrl.basScaleX = 1.0f;
        playerCtrl.transform.localScale = new Vector3(playerCtrl.basScaleX, 1.0f, 1.0f);
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if( !playerCtrl.jumped &&
            ( col.gameObject.tag == "Road" ||
              col.gameObject.tag == "MoveObject" ||
              col.gameObject.tag == "Enemy") )
        {
            playerCtrl.groundY = transform.parent.transform.position.y;
        }
    }
}
