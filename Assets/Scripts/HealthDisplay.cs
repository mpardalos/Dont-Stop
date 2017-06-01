using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour {
    public float MaxHealth; 
    public RectTransform FillTransform;

    public float Health {
        set {
            FillTransform.localScale = new Vector3(value/MaxHealth, 1, 1);
            Debug.Log(FillTransform.localScale);
        }
    }


    void Start() {
        Health = MaxHealth;
    }
            
}
