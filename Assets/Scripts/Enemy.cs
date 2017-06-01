using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    
    #region Inspector properties
    public float MoveSpeed;
    public float AttackDamage;
    public float DamageApplicationInterval;
    #endregion

    private GameMaster  m_GameMaster;
    private Player      m_Player;
    private Rigidbody2D m_Rigidbody;

    // Damage Application has to happen in the main thread
    private Timer m_DamageApplicationTimer;
    private int   m_PendingDamageApplications;

    void Start() {
        m_GameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        m_Player     = m_GameMaster.Player;
        m_Rigidbody  = GetComponent<Rigidbody2D>();

        m_DamageApplicationTimer           = new Timer();
        m_DamageApplicationTimer.Interval  = DamageApplicationInterval * 1000;
        m_DamageApplicationTimer.AutoReset = true;
        m_DamageApplicationTimer.Elapsed  += (source, e) => m_PendingDamageApplications++;

    }
	
    void Update() {
        // Move towards the player
        Vector2 playerDirection = (m_Player.transform.position - transform.position).normalized;
        m_Rigidbody.velocity    = playerDirection * MoveSpeed;

        // If Damage should be applied to the player, do it and reset the counter
        if (m_PendingDamageApplications != 0) {
            m_Player.SetHealth(m_Player.GetHealth() - m_PendingDamageApplications * AttackDamage,
                               Player.HealthChangeCause.Attack);
            m_PendingDamageApplications = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        // If we touched the player and have not already started the damage timer, start it,
        // and apply one attack immediately
        if (collision.gameObject == m_Player.gameObject && !m_DamageApplicationTimer.Enabled) {
            m_PendingDamageApplications++;
            m_DamageApplicationTimer.Start();
        }
    }
    
    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject == m_Player.gameObject) {
            m_DamageApplicationTimer.Stop();
        }
    }
}
