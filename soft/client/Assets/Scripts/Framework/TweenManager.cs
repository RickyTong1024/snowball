using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TweenManager : MonoBehaviour
{
    private List<GameObject> tween_obj = new List<GameObject>();

    public void Add_Tween_Postion(GameObject obj, float duration, Vector3 from, Vector3 to)
    {
        if (obj == null)
        {
            return;
        }
        TweenPosition tw = TweenPosition.Begin(obj, duration, from);
        if (from != null)
        {
            tw.from = from;
        }
        if (to != null)
        {
            tw.to = to;
        }
        tw.method = UITweener.Method.EaseInOut;
        tw.duration = duration;
        tw.delay = 0;
    }
    public void Add_Tween_Postion(GameObject obj, float duration, Vector3 from, Vector3 to, int effect, float delay)
    {
        if (obj == null)
        {
            return;
        }
        TweenPosition tw = TweenPosition.Begin(obj, duration, from);
        if (from != null)
        {
            tw.from = from;
        }
        if (to != null)
        {
            tw.to = to;
        }
        switch (effect)
        {
            case 0: tw.method = UITweener.Method.Linear; break;
            case 1: tw.method = UITweener.Method.EaseIn; break;
            case 2: tw.method = UITweener.Method.EaseOut; break;
            case 3: tw.method = UITweener.Method.EaseInOut; break;
            case 4: tw.method = UITweener.Method.BounceIn; break;
            case 5: tw.method = UITweener.Method.BounceOut; break;
            default: break;
        }
        tw.duration = duration;
        tw.delay = delay;
    }
    public void Add_Tween_Alpha(GameObject obj, float duration, float from, float to)
    {
        if (obj == null)
        {
            return;
        }
        TweenAlpha tw = TweenAlpha.Begin(obj, duration, from);
        tw.from = from;
        tw.to = to;
        tw.method = UITweener.Method.EaseInOut;
        tw.duration = duration;
        tw.delay = 0;
    }
    public void Add_Tween_Alpha(GameObject obj, float duration, float from, float to, int effect, float delay)
    {
        if (obj == null)
        {
            return;
        }
        TweenAlpha tw = TweenAlpha.Begin(obj, duration, from);
        tw.from = from;
        tw.to = to;
        switch (effect)
        {
            case 0: tw.method = UITweener.Method.Linear; break;
            case 1: tw.method = UITweener.Method.EaseIn; break;
            case 2: tw.method = UITweener.Method.EaseOut; break;
            case 3: tw.method = UITweener.Method.EaseInOut; break;
            case 4: tw.method = UITweener.Method.BounceIn; break;
            case 5: tw.method = UITweener.Method.BounceOut; break;
            default: break;
        }
        tw.duration = duration;
        tw.delay = delay;
    }
    public void Add_Tween_Scale(GameObject obj, float duration, Vector3 from, Vector3 to)
    {
        if (obj == null)
        {
            return;
        }
        TweenScale tw = TweenScale.Begin(obj, duration, from);
        tw.from = from;
        tw.to = to;
        tw.method = UITweener.Method.EaseInOut;
        tw.duration = duration;
        tw.delay = 0;
    }
    public void Add_Tween_Scale(GameObject obj, float duration, Vector3 from, Vector3 to, int effect, float delay)
    {
        if (obj == null)
        {
            return;
        }
        TweenScale tw = TweenScale.Begin(obj, duration, from);
        tw.from = from;
        tw.to = to;
        switch (effect)
        {
            case 0: tw.method = UITweener.Method.Linear; break;
            case 1: tw.method = UITweener.Method.EaseIn; break;
            case 2: tw.method = UITweener.Method.EaseOut; break;
            case 3: tw.method = UITweener.Method.EaseInOut; break;
            case 4: tw.method = UITweener.Method.BounceIn; break;
            case 5: tw.method = UITweener.Method.BounceOut; break;
            default: break;
        }
        tw.duration = duration;
        tw.delay = delay;
    }
}
