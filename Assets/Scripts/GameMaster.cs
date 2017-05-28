using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public HealthDisplay HealthDisplay;

    private Player m_Player;
    public Player Player {get; private set;}

	void Awake() {
	    Player = GameObject.Find("Player").GetComponent<Player>();	
	}

    void Start() {
        Player.HealthChanged += (newHealth) => HealthDisplay.Health = newHealth;
    }
}
