using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{

    private Button singlePlayer;
    private Button simpleCompanion;
    private Button fuzzyCompanion;
    private Button CalcPlayerModel;
    private GameManager manager;

    // Start is called before the first frame update
    void Start()
    {
        if (!manager)
        {
            manager = GameObject.FindObjectOfType<GameManager>();
        }
        Button[] buttons = GameObject.FindObjectsOfType<Button>();
        foreach(Button button in buttons)
        {
            if(button.name == "Single")
            {
                singlePlayer = button;
                singlePlayer.onClick.AddListener(singleLevel);
            }
            if(button.name == "FuzzyCompanion")
            {
                fuzzyCompanion = button;
                fuzzyCompanion.onClick.AddListener(fuzzyCompanionLevel);
            }
            if(button.name == "SimpleCompanion")
            {
                simpleCompanion = button;
                simpleCompanion.onClick.AddListener(simpleCompanionLevel);
            }
            if(button.name == "Calculate")
            {
                CalcPlayerModel = button;
                CalcPlayerModel.onClick.AddListener(calculatePlayerModel);
            }
        }
    }

    void singleLevel()
    {
        SceneManager.LoadScene("Level_1");
    }

    void fuzzyCompanionLevel()
    {
        SceneManager.LoadScene("Level_2");
    }
    
    void simpleCompanionLevel()
    {
        SceneManager.LoadScene("Level_3");
    }

    void calculatePlayerModel()
    {
        manager.KillTendency = (manager.KilledEnemies / manager.TotalEnemies) * 100;
        manager.Aggression = (0.5f + manager.detectedTime / manager.singleGameTime * 0.5f - manager.hidingTime / manager.singleGameTime * 0.5f) * 100f;
        TextMeshProUGUI[] texts = FindObjectsOfType<TextMeshProUGUI>();
        foreach(TextMeshProUGUI textField in texts)
        {
            if(textField.name == "KTValue")
            {
                textField.text = manager.KillTendency.ToString();
            }
            if(textField.name == "AValue")
            {
                textField.text = manager.Aggression.ToString();
            }
        }
    }

}
