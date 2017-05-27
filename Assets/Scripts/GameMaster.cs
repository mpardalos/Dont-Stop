using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public HealthDisplay HealthDisplay;

    private Player m_Player;

	void Start() {
	    m_Player = GameObject.Find("Player").GetComponent<Player>();	
        m_Player.HealthChanged += (newHealth) => HealthDisplay.Health = newHealth;
	}
}
