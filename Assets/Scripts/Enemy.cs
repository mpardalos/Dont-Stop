using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    
    #region Inspector properties
    public float MoveSpeed;
    #endregion

    private GameMaster m_GameMaster;
    private Player m_Player;
    private Rigidbody2D m_Rigidbody;

	void Start() {
        m_GameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        m_Player = m_GameMaster.Player;
        m_Rigidbody = GetComponent<Rigidbody2D>();
	}
	
	void Update() {
        Vector2 playerDirection = (m_Player.transform.position - transform.position).normalized;
        m_Rigidbody.velocity = playerDirection * MoveSpeed;
	}

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject == m_Player.gameObject) {
            m_Player.SetHealth(m_Player.GetHealth() - 1, Player.HealthChangeCause.Attack);
        }
    }
}
