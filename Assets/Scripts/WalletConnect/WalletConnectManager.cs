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
using Newtonsoft.Json;
using System.Linq;

public class WalletConnectManager : MonoBehaviorInstance<WalletConnectManager>
{
    public Action onPlayerUpdatePoint;
    private Action _onSuccess;

    [SerializeField] private GameManager _gameManager;
    private bool _isShowConnectWalletUI;
    string userAddress = "0x04Ce066AF4C50AEe8febCB7F856109A312abc2011877955eCd2db6b2bAd56d87";
    string contractAddress = "0x7bd89ba87f34b47facaeb4d408dadd1915d16a6c828d7ba55692eb705f0a5cc";

    protected override void ChildAwake()
    {
        base.ChildAwake();
        _isShowConnectWalletUI = false;
    }

    void Start()
    {
        SocketConnectManager.Instance.onClaim += Claim;
    }

    void OnDisable()
    {
        SocketConnectManager.Instance.onClaim -= Claim;
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
        SyncPlayerPoint();
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

        string[] calldata = new string[1];
        calldata[0] = userAddress;
        string calldataString = JsonUtility.ToJson(new ArrayWrapper { array = calldata });
        Debug.Log("data string: " + calldataString);
#if UNITY_EDITOR
        UnityRpcPlatform rpcPlatform = new UnityRpcPlatform();
        ContractInteraction contractInteraction = new ContractInteraction(contractAddress, "getUserPoint", userAddress);
        rpcPlatform.CallContract(contractInteraction, OnSuccess, OnError);
#elif UNITY_WEBGL
        JSInteropManager.CallContract(contractAddress, "getUserPoint", calldataString, gameObject.name, "Erc721Callback");
#endif
    }

    public void Erc721Callback(string response)
    {
        JsonResponse jsonResponse = JsonUtility.FromJson<JsonResponse>(response);
        BigInteger balance = BigInteger.Parse(jsonResponse.result[0].Substring(2), NumberStyles.HexNumber);
        Debug.Log("Balance: " + balance);
        PlayerDataManager.Instance.SetPlayerPoint((int)balance);
        _gameManager.UpdateCoin((int)balance);
    }
    public void Claim(ProofClass proofClass)
    {
        Settings.apiurl = "https://starknet-mainnet.public.blastapi.io/rpc/v0_7";
        string[] calldata = new string[2];
        calldata[0] = proofClass.point.ToString();
        calldata[1] = proofClass.timestamp.ToString();
        string proofArray = $",[\"{proofClass.proof[0]}, {proofClass.proof[1]}\"]";

        string callDataString = JsonUtility.ToJson(new ArrayWrapper{array = calldata});
        callDataString = callDataString.Replace("]}", "");
        callDataString = callDataString + proofArray + "]}";

        Debug.Log("callDataString: " + callDataString);
#if UNITY_EDITOR
        // UnityRpcPlatform rpcPlatform = new UnityRpcPlatform();
        // TransactionInteraction contractInteraction = new TransactionInteraction(userAddress, contractAddress, "rewardPoint", callDataString, CairoVersion.Version1, "0xa2d9d3b14c", "0x534e5f474f45524c49", );
        // rpcPlatform.CallContract(contractInteraction, OnSuccess, OnError);
#elif UNITY_WEBGL
        JSInteropManager.SendTransaction(contractAddress, "rewardPoint", callDataString, gameObject.name, "ClaimCallback");
#endif
    }

    public void ClaimCallback(string response)
    {
        Debug.Log("Response: " + response);
    }
}
