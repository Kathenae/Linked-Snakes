using System;
using System.Collections;
using UnityEngine;

public class Blink : MonoBehaviour
{
    public Color fromColor;
    public Color toColor;

    new MeshRenderer renderer;

    public void StarBlinking(Action OnFinish)
    {
        renderer = GetComponent<MeshRenderer>();
        StartCoroutine(StartBlink(OnFinish));
    }


    IEnumerator StartBlink( Action OnFinish)
    {
        float timer = 0;

        while(timer < 8)
        {
            timer += Time.deltaTime * 5;
            renderer.material.color = Color.Lerp(fromColor, toColor, Mathf.PingPong(timer, 1));
            yield return null;
        }

        if (OnFinish != null)
            OnFinish();

    }
}
