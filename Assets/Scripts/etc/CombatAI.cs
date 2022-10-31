using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAI : MonoBehaviour
{
    // 외부 파라미터, 인스펙터 표시
    public int freeAIMax = 3;
    public int blockAttackAIMax = 10;

    // 코드
    private void FixedUpdate()
    {
        var activeEnemyMainList = new List<EnemyMain>();

        // 카메라에 비치고 있는 적을 검색
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");

        if( enemyList == null )
        {
            return;
        }

        foreach( GameObject enemy in enemyList )
        {
            EnemyMain enemyMain = enemy.GetComponent<EnemyMain>();

            if( enemyMain != null )
            {
                if( enemyMain.combatAIOrder && enemyMain.cameraEnabled )
                {
                    activeEnemyMainList.Add(enemyMain);
                }
            }
            else
            {
                Debug.LogWarning(string.Format("CombatAI : EnemyMain null : {0} {1}", enemy.name, enemy.transform.position));
            }
        }

        // 공격하는 적을 억제
        int i = 0;

        foreach( EnemyMain enemyMain in activeEnemyMainList )
        {
            if( i < freeAIMax )
            {
                // 자유 행동
            }
            else if( i < freeAIMax + blockAttackAIMax )
            {
                // 공격 억제
                if( enemyMain.aiState == ENEMYAISTS.RUNTOPLAYER )
                {
                    enemyMain.SetCombatAIState(ENEMYAISTS.WAIT);
                }
            }
            else
            {
                // 행동 정지
                if( enemyMain.aiState != ENEMYAISTS.WAIT )
                {
                    enemyMain.SetCombatAIState(ENEMYAISTS.WAIT);
                }
            }
            i++;
        }

    }
}
