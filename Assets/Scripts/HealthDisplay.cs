using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour {
    public int MaxHealth;
    
    private int m_Health;
    public int Health {
        get {
            return m_Health;
        } 
        set {
            if (0 <= value && value <= MaxHealth) {
                m_Health = value;
                m_Display.text = m_Health.ToString();
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
