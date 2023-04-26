using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject bar;
    [HideInInspector]public float maxValue;

    [SerializeField] Enemy enemy; 

    float barMult;
    private void Awake()
    {

    }

    private void Start()
    {
        barMult = 1 / maxValue;
        enemy.OnDamageTaken += UpdateBar;
        ResetBar();
    }

    public void ResetBar()
    {
        bar.transform.localScale = new Vector3(1, bar.transform.localScale.y, bar.transform.localScale.z);
    }


    void UpdateBar(int value)
    {

        float xValue = value * barMult;
        if (value < 0) xValue = 0;
        bar.transform.localScale = new Vector3(xValue, bar.transform.localScale.y, bar.transform.localScale.z);
    }

    private void OnDestroy()
    {
        enemy.OnDamageTaken -= UpdateBar;
    }
}
