using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FOXFADE_STATE
{
    NON,
    IN,
    OUT
}

public class zFoxFadeFilter : MonoBehaviour
{
    public static zFoxFadeFilter instance = null;

    // 외부 파라미터, 인스펙터 표시
    public GameObject fadeFilterObject = null;
    public string attacheObject = "FadeFilterPoint";

    // 외부 파라미터
    [System.NonSerialized] public FOXFADE_STATE fadeState;

    // 내부 파라미터
    private float startTime;
    private float fadeTime;
    private Color fadeColor;

    // 코드
    private void Awake()
    {
        instance = this;
        fadeState = FOXFADE_STATE.NON;
    }

    void SetFadeAction(FOXFADE_STATE state, Color color, float time)
    {
        fadeState = state;
        startTime = Time.time;
        fadeTime = time;
        fadeColor = color;
    }

    public void FadeIn(Color color, float time)
    {
        SetFadeAction(FOXFADE_STATE.IN, color, time);
    }

    public void FadeOut(Color color, float time)
    {
        SetFadeAction(FOXFADE_STATE.OUT, color, time);
    }

    void SetFadeFilterColor(bool enabled, Color color)
    {
        if( fadeFilterObject )
        {
            fadeFilterObject.GetComponent<Renderer>().enabled = enabled;
            fadeFilterObject.GetComponent<Renderer>().material.color = color;
            SpriteRenderer sprite = fadeFilterObject.GetComponent<SpriteRenderer>();

            if( sprite )
            {
                sprite.enabled = enabled;
                sprite.color = color;
                fadeFilterObject.SetActive(enabled);
            }
        }
    }

    private void Update()
    {
        // 페이드 필터 적용 (씬 간 이동 대응)
        if( attacheObject != null )
        {
            GameObject go = GameObject.Find(attacheObject);
            fadeFilterObject.transform.position = new Vector3( go.transform.position.x, go.transform.position.y, 1 );
        }

        // 페이드 처리
        switch( fadeState )
        {
            case FOXFADE_STATE.NON:
                break;

            case FOXFADE_STATE.IN:
                fadeColor.a = 1.0f - ((Time.time - startTime) / fadeTime);

                if( fadeColor.a > 1.0f || fadeColor.a < 0.0f )
                {
                    fadeColor.a = 0.0f;
                    fadeState = FOXFADE_STATE.NON;
                    SetFadeFilterColor(false, fadeColor);
                    break;
                }
                SetFadeFilterColor(true, fadeColor);
                break;

            case FOXFADE_STATE.OUT:
                fadeColor.a = (Time.time - startTime) / fadeTime;

                if (fadeColor.a > 1.0f || fadeColor.a < 0.0f)
                {
                    fadeColor.a = 1.0f;
                    fadeState = FOXFADE_STATE.NON;
                    break;
                }
                SetFadeFilterColor(true, fadeColor);
                break;
        }
    }
}
