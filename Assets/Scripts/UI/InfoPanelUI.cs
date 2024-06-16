using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerAddress, _point, _sahPoint;
    [SerializeField] private Image _playerIcon;
    [SerializeField] private Button _claimBtn, _logOutBtn;

    #region Unity functions
    void Awake()
    {
        _claimBtn.onClick.AddListener(Claim);       
        _logOutBtn.onClick.AddListener(LogOut);
    }
    void Start()
    {
        // SocketConnectManager.Instance.onUpdateCoin += RefreshPoint;
    }
    void OnDestroy()
    {
        // SocketConnectManager.Instance.onUpdateCoin -= RefreshPoint;
    }
    #endregion

    public void RefreshUI()
    {
        if(PlayerDataManager.Instance.IsConnected())
        {
            string address = PlayerDataManager.Instance.GetPlayerAddress();
            _playerAddress.text = address;
        }
    }

    public void RefreshPoint(int point)
    {
        Debug.Log("RefreshPoint: " + point);
        _point.text = point.ToString();
    }

    private void Claim()
    {

    }
    private void LogOut()
    {
    }
    public void Show()
    {
        this.transform.DOScale(1, 0.4f).SetEase(Ease.OutCubic);
    }
    public void Hide()
    {
        this.transform.DOScale(0, 0.2f);
    }
}
