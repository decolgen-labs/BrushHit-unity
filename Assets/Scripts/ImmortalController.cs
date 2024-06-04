using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script for Immortal power-up
public class ImmortalController : MonoBehaviour
{
    [SerializeField] ParticleSystem _starVFXPrefabs;
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    //Check the collision with brush, destroy power-up, create VFX and give player the power-up
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Brush"))
        {
            Destroy(gameObject.transform.parent.gameObject);
            _gameManager.StartEffect(1);

            ParticleSystem starVFX = Instantiate(_starVFXPrefabs, transform.position, Quaternion.identity);
            var main = starVFX.gameObject.GetComponent<ParticleSystem>().main;
            Destroy(starVFX.gameObject, main.duration);
        }
    }
}
