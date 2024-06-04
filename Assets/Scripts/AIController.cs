using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script handle AI
public class AIController : MonoBehaviour
{
    [SerializeField] GameObject[] _brush;
    [SerializeField] Material _brushMaterial;
    [SerializeField] float _speed;

    private GameObject _mainBrush;
    private GameObject _secondBrush;
    private Animator _animator;
    private GameManager _gameManager;

    private int _brushIndex = 0;
    private int _direction;

    private bool _isTransition;

    void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _animator = GetComponent<Animator>();
        for (int i = 0; i < _brush.Length - 1; i++)
        {
            _brush[i].gameObject.GetComponent<Renderer>().material = _brushMaterial;
        }
        Reset();
    }

    public void Reset()
    {
        _brushIndex = 0;
        _direction = 1;
        _mainBrush = _brush[_brushIndex];
        _secondBrush = _brush[(_brushIndex + 1) & 2];
        IsSpawning(true);
    }

    // Update is called once per frame
    void Update()
    {
        GameObject thingBelow = CheckBelow();
        if (thingBelow != null && thingBelow.CompareTag("Platform"))
        {
            UpdateMainBrush();
        }
    }

    private void FixedUpdate()
    {
        if (!_isTransition && !_gameManager.IsFreezing)
        {
            transform.RotateAround(_mainBrush.transform.position, Vector3.up * _direction, Time.deltaTime * _speed);
        }
    }

    public void UpdateMainBrush()
    {
        _secondBrush = _mainBrush;
        _brushIndex = (_brushIndex + 1) % 2;
        _direction = -_direction;
        _mainBrush = _brush[_brushIndex];
    }

    IEnumerator EnableAnimators()
    {
        _isTransition = true;
        yield return new WaitForSeconds(2);
        _isTransition = false;
    }

    //Handle animations
    public void IsSpawning(bool spawn)
    {
        StartCoroutine(EnableAnimators());
        string nameTrigger = "Spawn";
        if (!spawn)
        {
            nameTrigger = "Dispawn";
        }
        _animator.SetTrigger(nameTrigger);
    }

    //Check below of brush to sure that AI not going out the platform
    public GameObject CheckBelow()
    {
        Ray ray = new Ray(_secondBrush.transform.position, Vector3.down);
        RaycastHit[] raycastHits = Physics.RaycastAll(ray, 100f);
        foreach (RaycastHit hit in raycastHits)
        {
            if (hit.transform.gameObject.CompareTag("Platform"))
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }
}
