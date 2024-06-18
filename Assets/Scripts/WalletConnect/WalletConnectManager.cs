using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NOOD;
using System;
using Utils;
using System.Numerics;
using System.Globalization;
using StarkSharp.Settings;
using StarkSharp.Connectors.Components;
using StarkSharp.Platforms.Unity.RPC;

public class WalletConnectManager : MonoBehaviorInstance<WalletConnectManager>
{
    public Action onPlayerUpdatePoint;
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

    public void CheckUserBalance(string userAddress, string contractAddress, string selector, UnityRpcPlatform rpcPlatform)
    {
        ContractInteraction contractInteraction = new ContractInteraction(contractAddress, selector, userAddress);
        rpcPlatform.CallContract(contractInteraction, OnSuccess, OnError);
    }
    void OnSuccess(string result)
    {
        Debug.Log("Contract call successful: " + result);
    }

    void OnError(string error)
    {
        Debug.LogError("Contract call error: " + error);
    }

    public void SyncPlayerPoint()
    {
        Settings.apiurl = "https://starknet-mainnet.public.blastapi.io/rpc/v0_7";
        string userAddress = "0x04Ce066AF4C50AEe8febCB7F856109A312abc2011877955eCd2db6b2bAd56d87";
        string contractAddress = "0x7bd89ba87f34b47facaeb4d408dadd1915d16a6c828d7ba55692eb705f0a5cc";

        string[] calldata = new string[1];
        calldata[0] = userAddress;
        string calldataString = JsonUtility.ToJson(new ArrayWrapper { array = calldata });
        // CheckUserBalance(userAddress, contractAddress, "getUserPoint", new UnityRpcPlatform());
        Debug.Log("SyncPlayerPoint");
        JSInteropManager.CallContract(contractAddress, "getUserPoint", calldataString, nameof(WalletConnectManager), nameof(Erc721Callback));
    }
    public void Erc721Callback(string response)
    {
        JsonResponse jsonResponse = JsonUtility.FromJson<JsonResponse>(response);
        BigInteger balance = BigInteger.Parse(jsonResponse.result[0].Substring(2), NumberStyles.HexNumber);
        Debug.Log("Balance: " + balance);
    }
}
