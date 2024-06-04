using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script for Grow up power-up
public class GrowupController : MonoBehaviour
{
    [SerializeField] ParticleSystem _starVFXPrefabs;
    private BrushController _brushController;
    private int _count = 0;

    private void Awake()
    {
        _brushController = GameObject.Find("BrushTool").GetComponent<BrushController>();
    }

    //Check the collision with brush, destroy power-up, create VFX and give player 1 point for the power-up
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Brush"))
        {
            _count++;
        }
        if (other.CompareTag("Brush") && _count == 1)
        {
            Destroy(gameObject.transform.parent.gameObject);
            _brushController.AddGrowUp();

            ParticleSystem starVFX = Instantiate(_starVFXPrefabs, transform.position, Quaternion.identity);
            var main = starVFX.gameObject.GetComponent<ParticleSystem>().main;
            Destroy(starVFX.gameObject, main.duration);
        }
    }
}
