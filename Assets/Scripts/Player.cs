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
    public int EnemyKillHealthIncrease;

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
    public event Action<int, HealthChangeCause> HealthChanged;
    public enum HealthChangeCause {
        HealthTick,
        Attack,
        Inspector,
        Initialization,
        EnemyKilled
    };

    private int m_Health;

    private Rigidbody2D m_Rigidbody;
    // Timer for ticking health
    private Timer m_HealthTick;
    private int m_HealthTicksSinceLastUpdate;
    private Animator m_Animator;

    void Start() {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();

        HealthChanged += OnHealthChanged;
        SetHealth(MaxHealth, HealthChangeCause.Initialization);

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

    public void SetHealth(int value, HealthChangeCause cause) {
        if (value < 0) {
            value = 0;
        } else if (value > MaxHealth) {
            value = MaxHealth;
        }
        m_Health = value;
        HealthChanged(m_Health, cause);
    }

    public int GetHealth() {
        return m_Health;
    }

    private void OnHealthChanged(int value, HealthChangeCause cause) {
        if (cause == HealthChangeCause.Attack) {
            m_Animator.Play("Attacked");
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
        // If we are currently attacking and the other gameobject is an enemy, destroy it.
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
                collision.gameObject.layer == LayerMask.NameToLayer("Enemies")) {
            Destroy(collision.gameObject);
            SetHealth(GetHealth() + EnemyKillHealthIncrease, HealthChangeCause.EnemyKilled);
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
