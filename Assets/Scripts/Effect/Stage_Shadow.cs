using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Shadow : MonoBehaviour
{
    public bool shadowEnabled = true;
    public bool updateEnabled = true;
    public Vector3 offsetPoition = new Vector3(-0.2f, 0.0f, 0.1f);
    public string sortingLayerName = "Char";
    public int sortingOrder = 0;
    public Color shadowColor = new Color(0.0f, 0.0f, 0.0f, 0.5f);

    SpriteRenderer spriteSrc;
    SpriteRenderer spriteCopy;


    // Start is called before the first frame update
    void Start()
    {
        spriteSrc = GetComponent<SpriteRenderer>();

        GameObject goEmpty = new GameObject("Shadow");
        goEmpty.AddComponent<SpriteRenderer>();
        spriteCopy = goEmpty.GetComponent<SpriteRenderer>();

        goEmpty.transform.parent = transform;
        goEmpty.transform.localScale = Vector3.one;

        spriteCopy.tag = "Shadow";
        spriteCopy.sortingLayerName = sortingLayerName;
        spriteCopy.color = shadowColor;

        UpdateShadow();
    }

    // Update is called once per frame
    void Update()
    {
        if(updateEnabled)
        {
            UpdateShadow();
        }
    }

    void UpdateShadow()
    {
        spriteCopy.transform.position = spriteSrc.transform.position;
        spriteCopy.transform.Translate(-0.2f, 0.0f, 0.1f, Space.Self);
        spriteCopy.sprite = spriteSrc.sprite;
    }
}
