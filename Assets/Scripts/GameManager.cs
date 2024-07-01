using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using NOOD;

//This script handle the game
public class GameManager : MonoBehaviour
{
    //This ScriptableObject contain the level data
    [SerializeField] LevelScriptableObject _levelScriptableObject;
    [SerializeField] SpawnManager _spawnManager;
    [SerializeField] BrushController _brushTool;
    [SerializeField] UIManager _uiManager;
    [SerializeField] AudioClip _scoreSFX;
    [SerializeField] AudioSource _audioSource;

    //The default color of rubber
    public Color DefaultColor;

    //The brushed color of rubber
    public Color BrushedColor;
    
    //Score to inc each rubber
    public int ScoreToIncrease;

    //Number of grow up point enough to get grow up power-up
    public int NumberOfGrowUps;

    //Ratio user can get a random power-ups
    public int PowerUpRatio;
    public bool IsPlaying { get; private set; }
    public bool IsTouchingDown { get; private set; } = false;
    public GameObject _levelData { get; private set; }
    public int Level { get; private set; }
    public int Stage { get; private set; }

    public bool IsFreezing { get; private set; }
    public bool IsImmortal { get; private set; }

    private bool _isTouching = false;
    private bool _isGetGrowUp = false;
    private bool _isGetFreeze = false;
    private bool _isGetImmortal = false;
    private int _currentPoint;
    private int _previousPoint;
    private int _totalRubberInLevel;

    #region Unity functions
    // Start is called before the first frame update
    void Start()
    {
        Physics.autoSyncTransforms = false;
        //this line to reduce the physics calculation
        Physics.reuseCollisionCallbacks = true;
        Init();
    }
    #endregion

    void Init()
    {
        //Get Player Save Data
        LoadLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0 || Input.GetMouseButtonDown(0)){
            if (!IsTouchingDown && !_isTouching)
            {
                IsTouchingDown = true;
                _isTouching = true;
            } else {
                IsTouchingDown = false;
            }
        }else if (Input.touchCount == 0 && !Input.GetMouseButtonDown(0)) {
            _isTouching = false;
            IsTouchingDown = false;
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.W))
        {
            WinGame();
            return;
        }
#endif

        if (IsPlaying)
        {
            CheckWin();
        } else {
            if (IsTouchingDown && !CheckClickUI() && !_brushTool._isTransition)
            {
#if UNITY_EDITOR
                Debug.Log("Pass Login");
                IsPlaying = true; 
                _uiManager.HideStartText();
                _brushTool.UpdateTag("Brush");
#elif UNITY_WEBGL
                WalletConnectManager.Instance.ConnectWallet(() => 
                { 
                    IsPlaying = true; 
                    _uiManager.HideStartText();
                    _brushTool.UpdateTag("Brush");
                });
#endif
            }
        }
        if(IsTouchingDown)
        {
            if (_currentPoint != 0 && _currentPoint >= _previousPoint)
            {
                GetExtraScore();
            }
            _previousPoint = _currentPoint;
            _currentPoint = 0;
        }
    }

    public void UpdatePoints()
    {
        _uiManager.UpdateScore();
    }

    public void UpdateSahCoin(int coin)
    {
        PlayerDataManager.Instance.SetPlayerSahPoint(coin);
        _uiManager.UpdateScore();
    }


    //this function to check when player click on UI so do not control the brush
    public bool CheckClickUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (var ray in raycastResults)
        {
            if (ray.gameObject.CompareTag("UI"))
            {
                return true;
            }
        }
        return false;
    }

    private void CheckWin()
    {
        GameObject[] rubbers = GameObject.FindGameObjectsWithTag("Rubber");
        int numberOfUncoloredRubber = 0;
        foreach (GameObject rubber in rubbers)
        {
            if(rubber.GetComponent<Renderer>().material.color == DefaultColor)
            {
                numberOfUncoloredRubber++;
                return;
            }
        }
        WinGame();
    }

    private void WinGame()
    {
        if(Stage == _levelScriptableObject.LevelDatas[Level].StagesData.Length - 1)
        {
            Level = (Level + 1) % _levelScriptableObject.LevelDatas.Length;
            Stage = 0;
        }
        else
        {
            Stage++;
        }
        StopGame();
        _uiManager.EndLevel(true);
        SaveData();
    }

    public void LoseGame()
    {
        StopGame();
        _uiManager.EndLevel(false);
    }

    public void RetryGame()
    {
        StopGame();
        _uiManager.RetryLevel();
    }

    public void LoadLevel()
    {
        _brushTool.transform.SetParent(null);

        Destroy(_levelData);
        _levelData = Instantiate(_levelScriptableObject.LevelDatas[Level].StagesData[Stage].Data);
        _isGetGrowUp = false;
        _isGetFreeze = false;
        _isGetImmortal = false;
        IsFreezing = false;
        IsImmortal = false;

        _brushTool.Reset();
        _brushTool.IsSpawning(true);

        DefaultColor = _levelScriptableObject.LevelDatas[Level].StagesData[Stage].DefaultColor;
        BrushedColor = _levelScriptableObject.LevelDatas[Level].StagesData[Stage].BrushedColor;
        
        _uiManager.StartLevel(Level, _levelScriptableObject.LevelDatas[Level].StagesData.Length, Stage);
        _uiManager.UpdateScore();

        StartCoroutine(SpawnRubbers());
    }

    IEnumerator SpawnRubbers()
    {
        yield return new WaitForEndOfFrame();
        _spawnManager.SpawnRubbers();
        _totalRubberInLevel = GameObject.FindGameObjectsWithTag("Rubber").Length;
    }

    private void StopGame()
    {
        IsPlaying = false;
        _brushTool.UpdateTag("Untagged");
        _brushTool.IsSpawning(false);
    }

    public void UpdateScore()
    {
        if (IsPlaying)
        {
            GetGrowUps();
            GetFreeze();
            GetImmortal();

            _uiManager.UpdateScore();
            _audioSource.PlayOneShot(_scoreSFX, 0.1f);
        }
    }

    private void GetGrowUps()
    {
        if (!_isGetGrowUp)
        {
            int result = UnityEngine.Random.Range(0, PowerUpRatio);
            if (result == 0)
            {
                _isGetGrowUp = true;
                _spawnManager.SpawnPowerUps(_brushTool.GetRotateBrush(), 0, 3);
            }
        }
    }

    private void GetFreeze()
    {
        if (!_isGetFreeze)
        {
            int result = UnityEngine.Random.Range(0, PowerUpRatio);
            if (result == 1)
            {
                _isGetFreeze = true;
                _spawnManager.SpawnPowerUps(_brushTool.GetRotateBrush(), 1, 1);
            }
        }
    }

    private void GetImmortal()
    {
        if (!_isGetImmortal)
        {
            int result = UnityEngine.Random.Range(0, PowerUpRatio);
            if (result == 2)
            {
                _isGetImmortal = true;
                _spawnManager.SpawnPowerUps(_brushTool.GetRotateBrush(), 2, 1);
            }
        }
    }

    private void GetExtraScore()
    {
        int extraScore = _currentPoint - _previousPoint;
        if(extraScore == 0)
        {
            return;
        }
        _uiManager.DisplayNotification(0, extraScore);
    }

    public void ChangeGameMode()
    {
        RetryGame();
    }

    public void StartEffect(int type)
    {
        if(type == 0)
        {
            StartCoroutine(FreezeEffect());
        } else {
            StartCoroutine(ImmortalEffect());
        }
    }

    IEnumerator FreezeEffect()
    {
        IsFreezing = true;
        _uiManager.ShowEffectUI(2);
        yield return new WaitForSeconds(5);
        IsFreezing = false;
    }

    IEnumerator ImmortalEffect()
    {
        IsImmortal = true;
        _uiManager.ShowEffectUI(3);
        yield return new WaitForSeconds(5);
        IsImmortal = false;
    }

    //Save data to PlayerPrefs (local)
    private void SaveData()
    {
        PlayerPrefs.Save();
    }
}
