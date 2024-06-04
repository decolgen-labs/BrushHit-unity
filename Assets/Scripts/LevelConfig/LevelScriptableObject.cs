using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelScriptableObject : ScriptableObject
{
    [Serializable]
    public class StageData
    {
        public GameObject Data;
        public Color DefaultColor;
        public Color BrushedColor;
    }

    [Serializable]
    public class LevelData
    {
        public StageData[] StagesData;
    }

    public LevelData[] LevelDatas;
}