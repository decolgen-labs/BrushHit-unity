using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BrushController : MonoBehaviour
{
    [SerializeField] GameObject[] _brush;
    [SerializeField] Material _brushMaterial;
    [SerializeField] Material _immortalMaterial;
    [SerializeField] ParticleSystem _brushVFX;
    [SerializeField] AudioClip _brushSFX;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] float _speed;
    [SerializeField] float _timeGrowUp;

    private GameObject _mainBrush;
    private GameManager _gameManager;
    private UIManager _uiManager;
    private CameraController _camera;

    private Vector3 _spawnPosition;
    private Vector3 _offset = new Vector3(0, 0.9f, 0);
    private Vector3 _oldParentPos;

    private int _brushIndex = 0;
    private int _numberOfGrowUp;
    private bool _isPlatform;
    private bool _isPressed;

    void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        _camera = GameObject.FindAnyObjectByType<CameraController>();
        SetMeshMaterial(_brushMaterial);
        JsSocketConnect.RegisterUpdateMainBrush(this.gameObject.name, nameof(UpdateMainBrush));
    }
    public void Reset()
    {
        Debug.Log("Reset");
        _brushIndex = 0;
        _numberOfGrowUp = 0;
        _mainBrush = _brush[_brushIndex];
        _camera.UpdateTarget(_mainBrush);
        UpdateTag("Untagged");
        _isPlatform = false;

        GameObject spawnPoint = GameObject.FindGameObjectWithTag("Spawn");
        if(spawnPoint != null)
        {
            _spawnPosition = spawnPoint.transform.position;
            // this.transform.position = _spawnPosition + _offset;
            transform.eulerAngles = Vector3.zero;
        }
        SocketConnectManager.Instance.UpdatePlatformOffset(Vector3.zero);
        if(spawnPoint != null)
        {
            SocketConnectManager.Instance.SetBrushPosition(_spawnPosition + _offset, GetRotateBrush().transform.position);
        }
        else
            SocketConnectManager.Instance.SetBrushPosition(_mainBrush.transform.position, GetRotateBrush().transform.position);
    }
    void Update()
    {
        if (_gameManager.IsPlaying)
        {
            if (_gameManager.IsTouchingDown && !_gameManager.CheckClickUI())
            {
                _isPressed = true;
                Debug.Log("Player input");
                SocketConnectManager.Instance.PlayerInput();
            }
        }

        // Update platform position
        if(_isPlatform)
        {
            Vector3 offset = this.transform.parent.position - _oldParentPos;
            SocketConnectManager.Instance.UpdatePlatformOffset(offset);
            _oldParentPos = this.transform.parent.position;
        }
        else
        {
            SocketConnectManager.Instance.UpdatePlatformOffset(Vector3.zero);
        }

        // Update position
        (Vector3 mainBrush, Vector3 otherBrush) = SocketConnectManager.Instance.GetBrushPosition();
        if(mainBrush != _mainBrush.transform.position)
        {
            if(_isPressed)
            {
                UpdateMainBrush();
            }
        }

        GetRotateBrush().transform.position = otherBrush;
        _mainBrush.transform.position = mainBrush;
        this.transform.position = mainBrush;

        // Update rotation
        Vector3 rotateVector = (otherBrush - mainBrush).normalized;
        this.transform.forward = rotateVector;
    }

    public void UpdateTag(string tag)
    {
        foreach (GameObject component in _brush)
        {
            component.tag = tag;
        }
    }

    public void SetImmortalEffect()
    {
        StartCoroutine(ImmortalEffect());
    }

    //Handle Immortal effect
    IEnumerator ImmortalEffect()
    {
        SetMeshMaterial(_immortalMaterial);
        yield return new WaitForSeconds(5);
        SetMeshMaterial(_brushMaterial);
    }

    private void SetMeshMaterial(Material material)
    {
        for (int i = 0; i < _brush.Length - 1; i++)
        {
            _brush[i].gameObject.GetComponent<Renderer>().material = material;
        }
    }

    public void UpdateMainBrush()
    {
        _brushIndex = (_brushIndex + 1) % 2;
        _mainBrush = _brush[_brushIndex];
        _camera.UpdateTarget(_mainBrush);
        _audioSource.PlayOneShot(_brushSFX, 1f);

        Debug.Log("Update main brush");
        GameObject thingBelow = CheckBelow();
        if (thingBelow == null || !thingBelow.CompareTag("Platform"))
        {
            _gameManager.LoseGame();
            transform.SetParent(null);
            _isPlatform = false;
        }
        else
        {
            transform.SetParent(thingBelow.transform.parent);
            _oldParentPos = thingBelow.transform.parent.position;
            _isPlatform = true;
        }
        _isPressed = false;
    }

    //Add point to grow up
    public void AddGrowUp()
    {
        if (_gameManager.IsPlaying)
        {
            _numberOfGrowUp++;
            UIGrowUpHandler.Ins.AddGrowUp();
            if (_numberOfGrowUp == 3)
            {
                GrowingUp();
            }
        }
    }

    //play animation and VFX of grow up power-ups
    private void GrowingUp()
    {
        if(_brushIndex == 0)
        {
            _brushVFX.Play();
        } else {

            _brushVFX.Play();
        }
    }

    public GameObject GetRotateBrush()
    {
        return _brush[(_brushIndex + 1) % 2];
    }

    //Check below of brush to know that player is going out the platform or not
    public GameObject CheckBelow()
    {
        Debug.Log("Check below");
        Ray ray = new Ray(_mainBrush.transform.position, Vector3.down);
        RaycastHit[] raycastHits = Physics.RaycastAll(ray, 100f);
        foreach(RaycastHit hit in raycastHits)
        {
            if (hit.transform.gameObject.CompareTag("Platform"))
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }
}
