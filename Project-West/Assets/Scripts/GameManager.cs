using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Manager
    {
        get; private set;
    }

    public float singleGameTime;
    public float hidingTime;
    public float detectedTime;
    public float KilledEnemies;
    public float TotalEnemies;

    public float KillTendency;
    public float Aggression;

    private void Awake()
    {
        if(Manager != null && Manager != this)
        {
            Destroy(this);
        }
        else
        {
            Manager = this;
            DontDestroyOnLoad(this.gameObject);
        }
        TotalEnemies = -1;
    }

    
    private void Update()
    {

        if (SceneManager.GetActiveScene().name == "Level_1")
        {
            if(TotalEnemies == -1)
            {
                EnemyController[] enemies = GameObject.FindObjectsOfType<EnemyController>();
                TotalEnemies = enemies.Length;
            }
            singleGameTime += Time.deltaTime;
        }

        if (SceneManager.GetActiveScene().name == "Level_2")
        {
            CompanionV2 companion = GameObject.FindObjectOfType<CompanionV2>();
            if (companion)
            {
                if (companion.Aggression != this.Aggression)
                {
                    companion.Aggression = this.Aggression;
                }
                if (companion.KillTendency != this.KillTendency)
                {
                    companion.KillTendency = this.KillTendency;
                }
            }
        }
    }

}
