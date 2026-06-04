using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingHandler : MonoBehaviour
{
    public static SettingHandler instance;
    [HideInInspector]
    public GameObject panel;

    private void Start()
    {
        panel = transform.GetChild(0).gameObject;
        instance = this;
    }
    
    public void Cross()
    {
       transform.GetChild(0).GetComponent<Animator>().Play("PanelOut");
    }
}
