using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script use to control the platform
public class PlatformController : MonoBehaviour
{
    //If this variable is check, the platform is the moving platform
    public bool IsMoving;

    //variable to define the scale of platform in X axis
    public int ScaleX;

    //variable to define the scale of platform in Z axis
    public int ScaleZ;

    [SerializeField] Transform _platformMesh;

    //If this is the moving platform, it will use this like a destination for moving
    [SerializeField] Transform _destinationObject;

    //The speed when platform moving
    [SerializeField] float _speed;

    //Range to simulate the moving of platform in Editor
    [SerializeField] [Range(0.0f, 1.0f)] float _movementSimulation;

    private float _scaleY = 0.2f;
    private Vector3 _sourcePosition;
    private Vector3 _destinationPosition;
    private float _offset = 0.1f;
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(IsMoving)
        {
            //if this is a moving platform, get the source and destination position
            _sourcePosition = transform.position;
            _destinationPosition = _destinationObject.transform.position;
        }
    }

    private void LateUpdate()
    {
        //if this is a moving platform and the game is not freezing by power-up, moving the platform
        if(IsMoving && !_gameManager.IsFreezing)
        {
            Vector3 direction = (_destinationPosition - _sourcePosition).normalized;
            transform.position += direction * _speed * Time.deltaTime;
            if (Vector3.Distance(transform.position, _destinationPosition) <= _offset)
            {
                direction = -direction;
                (_destinationPosition, _sourcePosition) = (_sourcePosition, _destinationPosition);
            }
        }
    }

    //This function use in Editor and Not in Playmode to simulate the movement of platform
    void OnValidate()
    {
        transform.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
        _platformMesh.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
        _platformMesh.localScale = new Vector3(ScaleX, _scaleY, ScaleZ);

        if (IsMoving)
        {
            _destinationObject.gameObject.SetActive(true);
            _destinationObject.hideFlags = HideFlags.None;
            MovePlatform();
        } else {
            _destinationObject.gameObject.SetActive(false);
            _destinationObject.hideFlags = HideFlags.HideInHierarchy;
        }
    }

    private void MovePlatform()
    {
        Vector3 direction = _destinationObject.position - transform.position;
        direction *= _movementSimulation;
        _platformMesh.position = transform.position + direction;
    }
}
