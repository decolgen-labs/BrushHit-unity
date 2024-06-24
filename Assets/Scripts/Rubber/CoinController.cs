using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NOOD;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    void Awake()
    {
        this.transform.rotation = Quaternion.Euler(90, 0, 0);
        MoveUpDownAnimation();
    }
    void Update()
    {
        this.transform.Rotate(0, 0, 100f * Time.deltaTime);
    }

    public void OnCoinCollect()
    {
        this.transform.DOKill();
        this.transform.DOMoveY(this.transform.position.y + 1f, 0.5f).SetEase(Ease.InBack);
        NoodyCustomCode.StartDelayFunction(() =>
        {
            this.transform.DOScale(0, 0.5f);
        }, 0.2f);
        SocketConnectManager.Instance.CoinCollect(this.transform.position);
    }

    private void MoveUpDownAnimation()
    {
        MoveUp();
    }
    private void MoveUp()
    {
        this.transform.DOMoveY(this.transform.position.y + 1f, 1f).SetEase(Ease.InOutFlash).OnComplete(() => 
        {
            MoveDown();
        });
    }
    private void MoveDown()
    {
        this.transform.DOMoveY(this.transform.position.y - 1f, 1f).SetEase(Ease.InOutFlash).OnComplete(() =>
        {
            MoveUp();
        });
    }
}
