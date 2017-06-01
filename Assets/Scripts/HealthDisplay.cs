using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour {
    public int MaxHealth; 
    public RectTransform FillTransform;

    public int Health {
        set {
            FillTransform.localScale = new Vector3((float)value/(float)MaxHealth, 1, 1);
            Debug.Log(FillTransform.localScale);
        }
    }


    void Start() {
        Health = MaxHealth;
    }
            
}
