using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameMaster : MonoBehaviour {

    #region Inspector Properties
    public HealthDisplay HealthDisplay;
    public GameObject EnemyPrefab;
    public bool MaintainInitialEnemyCount;
    [SerializeField]
    private int m_TargetEnemyCount; 
    #endregion

    private Player m_Player;
    public Player Player {get; private set;}

    private List<Enemy> m_Enemies;

    void Awake() {
	    Player = GameObject.Find("Player").GetComponent<Player>();	
	}

    void Start() {
        // Update the health on the display whenever the player's health changes
        Player.HealthChanged += (newHealth) => HealthDisplay.Health = newHealth;
        
        // Initialize the enemy list
        m_Enemies = new List<Enemy>();
        // Count the enemies in the Scene
        UpdateEnemies(); 
        // If we are maintaining the initial enemy count then set the target count to the
        // current count, otherwise, leave it at the value we got from the inspector
        m_TargetEnemyCount = MaintainInitialEnemyCount? m_Enemies.Count : m_TargetEnemyCount;
    }

    void Update() {
        UpdateEnemies();
        for (int i=0; i < (m_TargetEnemyCount - m_Enemies.Count); i++) {
            Instantiate(EnemyPrefab);
        }
    }

    private void UpdateEnemies() {
        // Reset the list and recount
        m_Enemies = new List<Enemy>();
        foreach (Object obj in FindObjectsOfType(typeof(Enemy))) {
            m_Enemies.Add(obj as Enemy);
        }
    }
}

[CustomEditor(typeof(GameMaster))]
[CanEditMultipleObjects]
public class GameMasterEditor : Editor {
    private bool m_ShowEnemyRespawnProperties = true;

    private SerializedProperty m_EnemyPrefab;
    private SerializedProperty m_MaintainInitialEnemyCount;
    private SerializedProperty m_TargetEnemyCount;
    private SerializedProperty m_HealthDisplay;

    void OnEnable() {
        m_HealthDisplay = serializedObject.FindProperty("HealthDisplay");

        m_EnemyPrefab = serializedObject.FindProperty("EnemyPrefab");
        m_MaintainInitialEnemyCount = serializedObject.FindProperty("MaintainInitialEnemyCount");
        m_TargetEnemyCount = serializedObject.FindProperty("m_TargetEnemyCount");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_HealthDisplay);

        m_ShowEnemyRespawnProperties = EditorGUILayout.Foldout(m_ShowEnemyRespawnProperties,
                "Enemy Respawn Properties");
        if (m_ShowEnemyRespawnProperties) {
            EditorGUILayout.PropertyField(m_EnemyPrefab);
            EditorGUILayout.PropertyField(m_MaintainInitialEnemyCount);

            // Enemy count only displayed if we are NOT maintaining the amount of enemies
            // initially in the scene
            EditorGUI.BeginDisabledGroup(m_MaintainInitialEnemyCount.boolValue);
                EditorGUILayout.PropertyField(m_TargetEnemyCount);
            EditorGUI.EndDisabledGroup();
        }

        serializedObject.ApplyModifiedProperties();
    }
}


