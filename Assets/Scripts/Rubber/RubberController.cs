using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RubberController : MonoBehaviour
{
    //Rigidbody to control the rubber Physics
    [SerializeField] Rigidbody _rubberRigidbody;

    //The VFX when the rubber is colored
    [SerializeField] ParticleSystem _rubberVFXPrefabs;

    //The Joint of rubber to platform
    [SerializeField] HingeJoint _hingeJoint;

    [SerializeField] GameObject _coinPrefab;

    private GameManager _gameManager;
    private Color _defaultColor;
    private Color _brushedColor;
    private CoinController _coinController;

    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        //The solverIterations determines how accurately Rigidbody joints and collision contacts are resolved
        _rubberRigidbody.solverIterations = 60;
    }
    void OnDestroy()
    {
        if(_coinController)
        {
            Destroy(_coinController.gameObject);
        }
    }

    //Set connected body of rubber's joint
    public void SetJoin(Rigidbody platform)
    {
        _hingeJoint.connectedBody = platform;
    }

    //Set color of rubber
    public void SetColor(Color color)
    {
        gameObject.GetComponent<Renderer>().material.color = color;
    }

    //Update the variable of color that use to handle the color of rubber
    public void UpdateColor(Color defaultColor, Color brushedColor)
    {
        _defaultColor = defaultColor;
        _defaultColor.a = 1;

        _brushedColor = brushedColor;
        _brushedColor.a = 1;
    }

    //Check the collision and Set color, Create VFX
    private void OnCollisionEnter(Collision collision)
    {
        Color color = gameObject.GetComponent<Renderer>().material.color;
        if ((collision.gameObject.CompareTag("Brush") || collision.contacts[0].otherCollider.transform.gameObject.CompareTag("Brush")) && color != _brushedColor)
        {
            SetColor(_brushedColor);
            CreateVFX(_brushedColor);
            gameObject.tag = "Untagged";
            _gameManager.UpdateScore();
        }
    }

    //Create a VFX at rubber position
    private void CreateVFX(Color color)
    {
        ParticleSystem rubberVFX = Instantiate(_rubberVFXPrefabs, transform.position, transform.rotation);
        var main = rubberVFX.gameObject.GetComponent<ParticleSystem>().main;
        main.startColor = color;
        Destroy(rubberVFX.gameObject, main.duration);
    }
}
