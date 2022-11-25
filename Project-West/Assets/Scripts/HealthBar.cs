using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Image bar;
    [SerializeField]
    private GameObject HealthOf;

    private void Awake()
    {
        bar = transform.Find("Bar").GetComponent<Image>();
    }

    private void Update()
    {
        float health = HealthOf.GetComponent<HealthComponent>().CurrentHealth;
        health = health / 100;
        bar.fillAmount = health;
    }
}
