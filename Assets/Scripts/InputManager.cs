using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    private Player m_Player;

	void Start () {
        m_Player = GameObject.Find("Player").GetComponent<Player>();		
	}
	
	void Update() {
        m_Player.Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));

        if (Input.GetButtonDown("Fire1")) {
            m_Player.Attack();
        }
	}
}
