using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour {
    public int MaxHealth;
    
    public int Health {
        get {
            return int.Parse(m_Display.text);
        } 
        set {
            if (0 <= value && value <= MaxHealth) {
                m_Display.text = value.ToString();
            } else {
                throw new ArgumentException("Health must be between 0 and MaxHealth");
            }
        }
    }

    private Text m_Display;

    void Start() {
        m_Display = gameObject.transform.Find("Display").gameObject.GetComponent<Text>();
        Health = MaxHealth;
    }
            
}
