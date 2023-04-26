using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiStatElement : MonoBehaviour
{
    public TMP_Text upgradeName;
    public GameObject bar;


    public void SetBarValue(float value)
    {
        bar.transform.localScale = new Vector3(value, bar.transform.localScale.y, bar.transform.localScale.z);
    }
}
