using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class MusicNoteBehavior : SkillBehavior
{
    float x;
    float y;
    public override void OnEnable()
    {
        x = Mathf.Abs(direction.x);
        y = Mathf.Abs(direction.y);
        if (x > y)
        {
            x = 0;
            y = 1;
        }
        else
        {
            x = 1;
            y = 0;
        }
        base.OnEnable();
        hitboxCollider.transform.DOLocalMove(new Vector3(x, y, 0), 0.75f).SetEase(Ease.InOutSine).SetLoops(1, LoopType.Yoyo).OnComplete(() =>
        {
            hitboxCollider.transform.DOLocalMove(new Vector3(-x, -y, 0), 0.75f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        });
    }
    public override void OnDisable()
    {
        hitboxCollider.transform.DOKill();
        hitboxCollider.transform.localPosition = new Vector3(0, 0, 0);
        base.OnDisable();
    }
}
