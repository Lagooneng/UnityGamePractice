using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_AfterImage : MonoBehaviour
{
    public SpriteRenderer spriteSrc;
    public bool afterImageEnabled;

    // Start is called before the first frame update
    void Start()
    {
        // afterImageEnabled = false;
        StartCoroutine("AfterImageUpdate");
    }

    IEnumerator AfterImageUpdate()
    {
        while(true)
        {
            if( !afterImageEnabled )
            {
                break;
            }

            // 잔상 게임 오브젝트 작성
            SpriteRenderer spriteCopy = Instantiate(spriteSrc) as SpriteRenderer;
            spriteCopy.transform.position = spriteSrc.transform.position;
            spriteCopy.transform.localScale = spriteSrc.transform.parent.transform.localScale;
            spriteCopy.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            spriteCopy.sortingLayerName = "Char";
            spriteCopy.sortingOrder = 1;
            spriteCopy.GetComponent<Stage_Shadow>().enabled = false;
            SpriteRenderer[] spList = spriteCopy.GetComponentsInChildren<SpriteRenderer>();

            foreach(SpriteRenderer sp in spList)
            {
                if( sp.name == "Shadow" )
                {
                    sp.enabled = false;
                }
            }

            Destroy(spriteCopy.gameObject, 0.3f);
            yield return new WaitForSeconds(0.05f);
        }
        // yield return new WaitForSeconds(1.0f);
    }
}
