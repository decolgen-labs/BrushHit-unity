using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//This script use to display Level Stage UI
public class LevelDisplayHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _currentLevelText;
    [SerializeField] TextMeshProUGUI _nextLevelText;
    [SerializeField] ToggleHandler _stagePrefab;
    [SerializeField] Transform _stageContainer;

    private List<ToggleHandler> stages = new List<ToggleHandler>();

    public void DisplayLevel(int level, int numberOfStages, int stage)
    {
        //Destroy the previous stages when display level again
        if(stages.Count != 0)
        {
            stages.ForEach(stage => Destroy(stage.gameObject));
            stages.Clear();
        }

        //Display level text and next level text
        _currentLevelText.text = level.ToString();
        _nextLevelText.text = (level + 1).ToString();

        //for each stage in this level, create a new stagePrefab and display that shows user know that level they are playing on
        for(int i = 0; i < numberOfStages; i++)
        {
            ToggleHandler toggle = Instantiate(_stagePrefab, _stageContainer, false);
            if (i <= stage)
            {
                toggle.OnToggle();
            }
            stages.Add(toggle);
        }

        //this line to rebuild the layout of UI because Instantiate in frame may cause the layout of UI display is wrong
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }
}
