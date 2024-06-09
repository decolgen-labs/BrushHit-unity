using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script used to spawn all rubbers on platform and power-up prefab
//rubber: all the small things on the platform you see in levels when you control the brush go through and change their color to get point
public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject _spawnPrefab;
    [SerializeField] GameObject _growUpPrefab;
    [SerializeField] GameObject _freezePrefab;
    [SerializeField] GameObject _immortalPrefab;
    [SerializeField] GameManager _gameManager;
    List<GameObject> _rubbers = new List<GameObject>();

    public void SpawnRubbers()
    {
        GameObject rubbers = new GameObject();
        rubbers.transform.SetParent(_gameManager._levelData.transform);
        rubbers.name = "Rubbers";
        rubbers.transform.position = Vector3.zero;

        //Find all platform in level
        
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        foreach (var platform in platforms)
        {
            Rigidbody platformRb = platform.GetComponent<Rigidbody>();
            //Get the size of rubber
            float offset = _spawnPrefab.transform.localScale.x;
            //Get X range and Z range of platform
            float rangeX = (platform.transform.localScale.x / 2) - (offset / 2);
            float rangeZ = (platform.transform.localScale.z / 2) - (offset / 2);

            //Get the max and min position range in X and Z axis
            float xRangeMax = platform.transform.position.x + rangeX;
            float xRangeMin = platform.transform.position.x - rangeX;
            float zRangeMax = platform.transform.position.z + rangeZ;
            float zRangeMin = platform.transform.position.z - rangeZ;

            //Spawn rubbers
            for (float i=xRangeMin; i<=xRangeMax; i += offset)
            {
                for (float j = zRangeMin; j <= zRangeMax; j += offset)
                {
                    Vector3 position = new Vector3(i, 0.5f, j);
                    GameObject rubber = Instantiate(_spawnPrefab, position, Quaternion.identity);
                    _rubbers.Add(rubber);
                    RubberController rubberController = rubber.GetComponent<RubberController>();
                    rubberController.SetJoin(platformRb);
                    rubberController.SetColor(_gameManager.DefaultColor);
                    rubberController.UpdateColor(_gameManager.DefaultColor, _gameManager.BrushedColor);
                    rubber.transform.parent = rubbers.transform;
                    rubber.GetComponent<RubberController>().index = _rubbers.Count -1;
                }
            }
        }
    }

    //This function used to spawn PowerUps in game, when user color the rubber, they can randomly spawn a power ups in there
    //The power ups always spawn in the constant radius from that colored rubber
    public void SpawnPowerUps(GameObject rubber, int type, int number)
    {
        Vector3 position = rubber.transform.position;
        float angle = 360 / (number + 1);
        for(int i = 0; i < number; i++)
        {
            GameObject powerUp = null;
            if (type == 0)
            {
                powerUp = Instantiate(_growUpPrefab, position, Quaternion.Euler(0, (i + 1) * angle, 0));
            } else if(type == 1) {
                powerUp = Instantiate(_freezePrefab, position, Quaternion.Euler(0, (i + 1) * angle, 0));
            } else {
                powerUp = Instantiate(_immortalPrefab, position, Quaternion.Euler(0, (i + 1) * angle, 0));
            }
            powerUp.transform.SetParent(_gameManager._levelData.transform);
        }
    }
}
