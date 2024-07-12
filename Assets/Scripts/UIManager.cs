using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Events
    public Action onArgentXButtonPress;
    public Action onBraavosButtonPress;
    #endregion

    #region Static
    public static UIManager Ins;
    #endregion

    #region Variables
    //Text to display notification
    [SerializeField] TextMeshProUGUI _titleText;

    //Canvas display when level is ended;
    [SerializeField] Canvas _endLevelCanvas;

    //Transition canvas
    [SerializeField] Canvas _transitionCanvas;

    // Player Info UI
    [SerializeField] InfoPanelUI _infoPanelUI;
    [SerializeField] Button _infoButton;

    //Component to display the stage of level
    [SerializeField] LevelDisplayHandler _levelDisplayHandler;
    [SerializeField] GameManager _gameManager;

    // Connect wallet UI
    [SerializeField] Button _connectWalletButtonArgentX;
    [SerializeField] Button _connectWalletButtonBraavos;
    [SerializeField] GameObject _connectWalletPanel;

    // Sound Btn
    [SerializeField] private ToggleHandler _soundToggle;

    public bool IsTransitioning { get; private set; } = false;

    private TextMeshProUGUI _scoreText;
    private TextMeshProUGUI _startText;
    private TextMeshProUGUI _notificationText;

    //Component to handle animation of notifications
    private Animator _notificationTextAnimator;
    
    //List of image display grow up stats
    private List<Image> growUpImages = new List<Image>();
    
    //Full size image to display whole screen effect
    private Image _effectImage;
    private bool _isShowPlayerInfo;
    #endregion

    #region Unity functions
    private void Awake()
    {
        //Create singleton
        if (Ins)
        {
            Destroy(Ins);
        }
        Ins = this;
        DontDestroyOnLoad(Ins);
    }
    private void Start()
    {
        //Start transition
        _transitionCanvas.gameObject.SetActive(true);
        //Display game ui
        StartCoroutine(DisplayGameUI());
        _infoButton.onClick.AddListener(ShowPlayerInfo);
        _connectWalletButtonArgentX.onClick.AddListener(() => 
        { 
            onArgentXButtonPress?.Invoke();
            UnityEngine.Debug.Log("ArgentX");
        });
        _connectWalletButtonBraavos.onClick.AddListener(() => 
        { 
            onBraavosButtonPress?.Invoke();
            UnityEngine.Debug.Log("Braavos");
        });
        _soundToggle.onToggle += OnSoundToggle;
    }
    #endregion
    private void OnSoundToggle(bool value)
    {
        foreach(var sound in FindObjectsOfType<AudioSource>())
        {
            sound.mute = !value;
        }
    }

    #region Connect Wallet UI
    public void ShowConnectWalletUI()
    {
        _connectWalletPanel.transform.localScale = Vector3.zero;
        _connectWalletPanel.SetActive(true);
        _connectWalletPanel.transform.DOScale(1, 0.5f).SetEase(Ease.OutCubic);
    }
    public void HideConnectWalletUI()
    {
        _connectWalletPanel.transform.DOScale(0, 0.5f).SetEase(Ease.OutCubic).onComplete += () => 
            _connectWalletPanel.SetActive(false);
    }
    #endregion

    //When user press screen to play -> hide the instruction
    public void HideStartText()
    {
        _startText.gameObject.SetActive(false);
    }

    //When game has notification for user, show effect and notification text
    public void ShowEffectUI(int type)
    {
        DisplayNotification(type, 0);
        StartCoroutine(ShowEffect(type));
    }

    IEnumerator ShowEffect(int type)
    {
        if(type == 2) //When user get Freeze Power-up
        {
            _effectImage.color = Color.blue;
        } else { //When user get Immortal Power-up
            _effectImage.color = Color.green;
        }

        _effectImage.color = new Color(_effectImage.color.r, _effectImage.color.g, _effectImage.color.b, 0.2f);
        _effectImage.enabled = true;
        yield return new WaitForSeconds(5);
        _effectImage.enabled = false;
    }

    //Function to update score in UI
    public void UpdateScore()
    {
        if(_scoreText != null)
        {
            _scoreText.text = PlayerDataManager.Instance.GetPlayerIngamePoint().ToString();
        }
        _infoPanelUI.RefreshPoint(PlayerDataManager.Instance.GetPlayerIngamePoint(), PlayerDataManager.Instance.GetPlayerSahPoint());
    }

    //Function to edit notification text
    public void DisplayNotification(int type, int extraInfo)
    {
        if (type == 0)
        {
            _notificationText.color = Color.yellow;
            _notificationText.text = "Combo " + extraInfo;
        } else if (type == 1) {
            _notificationText.color = Color.red;
            _notificationText.text = "GROWUP!!!";
        } else if (type == 2) {
            _notificationText.color = Color.blue;
            _notificationText.text = "FREEZING!!!";
        } else if (type == 3) {
            _notificationText.color = Color.green;
            _notificationText.text = "IMMORTAL!!!";
        }

        _notificationTextAnimator.SetTrigger("Show");
    }

    //When start level, turn off EndLevelCanvas and display Level stage in UI
    public void StartLevel(int level, int numberOfStage, int stage)
    {
        StartCoroutine(TransitionLevel(0, 0));
        _endLevelCanvas.gameObject.SetActive(false);
        _levelDisplayHandler.DisplayLevel(level, numberOfStage, stage);
    }

    //This function calls when user lose or win the level
    public void EndLevel(bool isWinning)
    {
        StartCoroutine(TransitionLevel(1, 1));
        DisplayEndCanvas(isWinning);
    }

    //This function calls when user retry the level
    public void RetryLevel()
    {
        StartCoroutine(TransitionLevel(1, 1));
    }

    //Turn on Transition Canvas and Reload Level
    IEnumerator TransitionLevel(float timeToWait, float duration)
    {
        IsTransitioning = true;
        yield return new WaitForSeconds(timeToWait);
        _transitionCanvas.GetComponent<Animator>().SetTrigger("Transition");
        yield return new WaitForSeconds(duration);
        if (timeToWait > 0)//timeToWait == 0 when the game start for the first time
        {
            _gameManager.LoadLevel();
        }
        IsTransitioning = false;
    }

    //When user end level->display notification
    public void DisplayEndCanvas(bool isWinning)
    {
        _endLevelCanvas.gameObject.SetActive(true);

        string text;
        Color color;
        if (isWinning)
        {
            text = "Congratulations";
            color = Color.yellow;
            AudioManager.Ins.PlayWinSFX();
        } else {
            text = "Oops~ You lose";
            color = Color.red;
            AudioManager.Ins.PlayLoseSFX();
        }

        _titleText.text = text;
        _titleText.color = color;
        _titleText.GetComponent<Animator>().SetTrigger("ShowTitle");
    }

    IEnumerator DisplayGameUI()
    {
        yield return new WaitForSeconds(0.5f);

        //Get components
        _scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        _notificationText = GameObject.Find("NotificationText").GetComponent<TextMeshProUGUI>();
        _startText = GameObject.Find("StartText").GetComponent<TextMeshProUGUI>();

        _notificationTextAnimator = _notificationText.GetComponent<Animator>();
        _effectImage = GameObject.Find("EffectImage").GetComponent<Image>();
        _infoPanelUI.RefreshUI();

        for (int i = 1; i <= 3; i++)
        {
            growUpImages.Add(GameObject.Find("GrowUp_" + i).GetComponent<Image>());
        }

        yield return new WaitForEndOfFrame();
        UpdateScore();
    }

    public void ShowPlayerInfo()
    {
        _isShowPlayerInfo = !_isShowPlayerInfo;
        if(_isShowPlayerInfo)
        {
            _infoPanelUI.Show();
        }
        else
        {
            _infoPanelUI.Hide();
        }
    }

    public void UpdateInfoPanel()
    {
        _infoPanelUI.RefreshUI();
    }
}
