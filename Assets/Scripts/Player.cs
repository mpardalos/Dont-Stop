using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour {
    // Inspector properties
    public float MoveSpeed;
    public int MaxHealth;

    // Fired when the player's health changes. Primarily used to notify GameMaster so that
    // it can update the health display
    public event Action<int> HealthChanged;

    private int m_Health;
    public int Health {
        get { 
            return m_Health;
        }

        set {
            if (0 <= value && value <= MaxHealth) {
                m_Health = value;
                HealthChanged(m_Health);
            } else {
                throw new ArgumentException("Health must be between 0 and MaxHealth");
            }
        }
    }

    private Rigidbody2D m_Rigidbody;

    void Start() {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Health = MaxHealth;
    }
    
    public void Move(Vector2 direction) {
        m_Rigidbody.velocity = direction.normalized * MoveSpeed;
    }

    void OnCollisionEnter2D(Collision2D col) {
        Health -= 1;
    }
}

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor {
    public override void OnInspectorGUI() {

        DrawDefaultInspector();
        
        #region +/- health buttons
        GUILayout.BeginHorizontal();

        if (Application.isPlaying && GUILayout.Button("-Health")) {
            ((Player)target).Health -= 1;
        }

        if (Application.isPlaying && GUILayout.Button("+Health")) {
            ((Player)target).Health += 1;
        }

        GUILayout.EndHorizontal();
        #endregion
    }
}


