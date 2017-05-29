using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour {

    #region Inspector properties
    public float MoveSpeed;
    public int MaxHealth;

    // better organisation in inspector
    [Serializable]
    public class _HealthTickProperties {
        public bool DoHealthTick;
        public int HealthTicksPerSecond;
        public int HealthLossPerTick;
    }
    public _HealthTickProperties HealthTickProperties;
    #endregion

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
    // Timer for ticking health
    private Timer m_HealthTick;
    private int m_HealthTicksSinceLastUpdate;
    private Animator m_Animator;

    void Start() {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        m_Health = MaxHealth;

        // The Health setter is not thread safe so the timer adds to a tick count which is
        // applied and reset in Update()
        if (HealthTickProperties.DoHealthTick) {
            m_HealthTick = new Timer((1.0/HealthTickProperties.HealthTicksPerSecond) * 1000);
            m_HealthTick.AutoReset = true;
            m_HealthTick.Elapsed += (source, e) => m_HealthTicksSinceLastUpdate += 1;
            m_HealthTick.Enabled = true;
        }
    }
    
    public void Move(Vector2 direction) {
        m_Rigidbody.velocity = direction.normalized * MoveSpeed;
    }

    public void Attack() {
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")){
            m_Animator.Play("Attack");
        }
    }

    void Update() {
        if (HealthTickProperties.DoHealthTick) {
            // Apply the health ticks from m_HealthTick 
            Health -= m_HealthTicksSinceLastUpdate * HealthTickProperties.HealthLossPerTick;
            m_HealthTicksSinceLastUpdate = 0;
        }
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
