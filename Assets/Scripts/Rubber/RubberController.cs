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

    public int index;
    private GameManager _gameManager;
    private Color _defaultColor;
    private Color _brushedColor;
    private Vector3 _worldPosition;
    public bool check;
    private bool _isCoin;
    private CoinController _coinController;

    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        //The solverIterations determines how accurately Rigidbody joints and collision contacts are resolved
        _rubberRigidbody.solverIterations = 60;
    }
    private void Update()
    {
        if(check)
        {
            // Debug.Log("Rubber: " + _worldPosition);
            // SocketConnectManager.Instance.IsBetweenBrush(index, _worldPosition, () => {
            //     _gameManager.IncreaseScore();

            //     SetColor(_brushedColor);
            //     CreateVFX(_brushedColor);
            //     gameObject.tag = "Untagged";
            // });
        }
    }
    void OnDestroy()
    {
        if(_coinController)
        {
            Destroy(_coinController.gameObject);
        }
    }

    public void SetCoin(bool isCoin)
    {
        _isCoin = isCoin;
        if(isCoin)
        {
            Debug.Log("Is Coin");
            _coinController = Instantiate(_coinPrefab, this.transform.position + new Vector3(0, 1f, 0), Quaternion.identity).GetComponent<CoinController>();
        }
    }

    //Set connected body of rubber's joint
    public void SetJoin(Rigidbody platform)
    {
        _hingeJoint.connectedBody = platform;
        _worldPosition = this.transform.position;
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
        if (check) return;
        Color color = gameObject.GetComponent<Renderer>().material.color;
        if ((collision.gameObject.CompareTag("Brush") || collision.contacts[0].otherCollider.transform.gameObject.CompareTag("Brush")) && color != _brushedColor)
        {
            SetColor(_brushedColor);
            CreateVFX(_brushedColor);
            gameObject.tag = "Untagged";
            if(_coinController != null)
            {
                _coinController.OnCoinCollect();
            }
            _gameManager.IncreaseScore();
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
