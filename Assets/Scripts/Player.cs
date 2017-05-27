using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float MoveSpeed;

    private Rigidbody2D m_Rigidbody;

    void Start() {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }
    
    public void Move(Vector2 direction) {
        Debug.Log(direction.normalized * MoveSpeed);
        m_Rigidbody.velocity = direction.normalized * MoveSpeed;
    }
}
