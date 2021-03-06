﻿using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour {

    #region Inspector properties
    public float MoveSpeed;
    public float MaxHealth;
    public float EnemyKillHealthIncrease;

    // better organisation in inspector
    [Serializable]
    public class _HealthTickProperties {
        public bool  DoHealthTick;
        public int   HealthTicksPerSecond;
        public float HealthLossPerTick;
    }
    public       _HealthTickProperties HealthTickProperties;
    #endregion

    // Fired when the player's health changes. Primarily used to notify GameMaster so that
    // it can update the health display
    public event Action<float, HealthChangeCause> HealthChanged;
    public enum  HealthChangeCause {
        HealthTick,
        Attack,
        Inspector,
        Initialization,
        EnemyKilled
    };

    private float m_Health;

    private Rigidbody2D m_Rigidbody;
    // Timer for ticking health
    private Timer       m_HealthTick;
    private int         m_HealthTicksSinceLastUpdate;
    private Animator    m_Animator;

    void Start() {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Animator  = GetComponent<Animator>();

        HealthChanged += OnHealthChanged;
        SetHealth(MaxHealth, HealthChangeCause.Initialization);

        // The Health setter is not thread safe so the timer adds to a tick count which is
        // applied and reset in Update()
        if (HealthTickProperties.DoHealthTick) {
            m_HealthTick           = new Timer();

            m_HealthTick.Interval  = (1.0/HealthTickProperties.HealthTicksPerSecond) * 1000;
            m_HealthTick.AutoReset = true;
            m_HealthTick.Elapsed  += (source, e) => m_HealthTicksSinceLastUpdate += 1;

            m_HealthTick.Start();
        }
    }
    
    void Update() {
        if (HealthTickProperties.DoHealthTick) {
            // Apply the health ticks from m_HealthTick 
            SetHealth(GetHealth() - m_HealthTicksSinceLastUpdate * HealthTickProperties.HealthLossPerTick, HealthChangeCause.HealthTick);
            m_HealthTicksSinceLastUpdate = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        bool currentlyAttacking = m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
        bool otherIsEnemy       = collision.gameObject.layer == LayerMask.NameToLayer("Enemies");

        if (currentlyAttacking && otherIsEnemy) {
            // Destroy the enemy and add the appropriate amount of Health
            Destroy(collision.gameObject);
            SetHealth(GetHealth() + EnemyKillHealthIncrease, HealthChangeCause.EnemyKilled);
        }
    }

    public void Move(Vector2 direction) {
        m_Rigidbody.velocity = direction.normalized * MoveSpeed;
    }

    public void Attack() {
        // Only Attack if currently idle
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")){
            m_Animator.Play("Attack");
        }
    }

    public void SetHealth(float value, HealthChangeCause cause) {
        // Clamp the new value between 0 and MaxHealth
        value = Math.Max(value, 0);
        value = Math.Min(value, MaxHealth);
        
        m_Health = value;
        HealthChanged(m_Health, cause);
    }

    public float GetHealth() {
        return m_Health;
    }

    private void OnHealthChanged(float value, HealthChangeCause cause) {
        if (cause == HealthChangeCause.Attack) {
            m_Animator.Play("Attacked");
        }
    }
}

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor {
    public override void OnInspectorGUI() {
        Player player = (Player) target;

        DrawDefaultInspector();
        
        #region +/- health buttons
        GUILayout.BeginHorizontal();

        if (Application.isPlaying && GUILayout.Button("-Health")) {
            player.SetHealth(player.GetHealth() - 1, Player.HealthChangeCause.Inspector);
        }

        if (Application.isPlaying && GUILayout.Button("+Health")) {
            player.SetHealth(player.GetHealth() + 1, Player.HealthChangeCause.Inspector);
        }

        GUILayout.EndHorizontal();
        #endregion
    }
}
