using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NOOD;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private bool _isClaimed = false;
    
    void Awake()
    {
    }
    void Update()
    {
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Brush") && _isClaimed == false)
        {
            _isClaimed = true;
            CollectCoin(this.transform.position);
        }
    }

    public void CollectCoin(Vector3 position)
    {
        this.transform.DOKill();
        this.transform.DOMoveY(this.transform.position.y + 1f, 0.5f).SetEase(Ease.InBack);
        NoodyCustomCode.StartDelayFunction(() =>
        {
            this.transform.DOScale(0, 0.5f);
        }, 0.2f);
        SocketConnectManager.Instance.CoinCollect(position);
    }
}
