using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NOOD;
using System;

public class WalletConnectManager : MonoBehaviorInstance<WalletConnectManager>
{
    private Action _onSuccess;
    private bool _isShowConnectWalletUI;

    protected override void ChildAwake()
    {
        base.ChildAwake();
        _isShowConnectWalletUI = false;
    }

    public void ConnectWallet(Action onSuccess)
    {
        if(JSInteropManager.IsConnected())
        {
            onSuccess?.Invoke();
        }
        else
        {
            _onSuccess = onSuccess;
            OpenConnectWalletPanel();
        }
    }
    public void OpenConnectWalletPanel()
    {
        if(_isShowConnectWalletUI == false)
        {
            _isShowConnectWalletUI = true;
            UIManager.Ins.ShowConnectWalletUI();
            UIManager.Ins.onArgentXButtonPress += ConnectArgentX;
            UIManager.Ins.onBraavosButtonPress += ConnectBraavos;
            _onSuccess += () =>
            {
                UIManager.Ins.HideConnectWalletUI();
                _isShowConnectWalletUI = false;
            };
        }
    }

    private void ConnectBraavos()
    {

    }

    private void ConnectArgentX()
    {
        StartCoroutine(ConnectWalletAsync(JSInteropManager.ConnectWalletArgentX));
    }
    IEnumerator ConnectWalletAsync(Action connectWalletFunction)
    {
        // Call the JavaScript method to connect the wallet
        connectWalletFunction();
        yield return new WaitUntil(() => JSInteropManager.IsConnected());

        _onSuccess?.Invoke();
        string playerAddress = JSInteropManager.GetAccount();
        PlayerDataManager.Instance.SetPlayerData(playerAddress);
        UIManager.Ins.UpdateInfoPanel();
    }
}
