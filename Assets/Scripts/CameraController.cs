using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector3 _offset;
    [SerializeField] float _speedTransition;
    private GameObject _target;

    // Move the camera follow the player
    void Update()
    {
        if(_target != null)
        {
            transform.position = Vector3.Lerp(transform.position, _target.transform.position + _offset, _speedTransition);
        }
    }

    public void UpdateTarget(GameObject target)
    {
        _target = target;
    }
}
