using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class for Toggle Component
public class ToggleHandler : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] Sprite[] _sprites;
    public Action<bool> onToggle;

    private int _index = 0;

    private void Awake()
    {
        _image.sprite = _sprites[0];
    }

    //Change the sprite of image
    public void OnToggle()
    {
        _image.sprite = _sprites[++_index % 2];
        onToggle?.Invoke(_index % 2 == 0);
    }
}
