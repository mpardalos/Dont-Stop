using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerTest {
    [RequireComponent(typeof(EdgeCollider2D))]
    public class ViewportCollider : MonoBehaviour {
        
        public bool AddBottomEdge = false; // Whether to add an edge to the botton of the screen

        private EdgeCollider2D m_Collider;

        void Start () {
            // Find the collider
            m_Collider = GetComponent<EdgeCollider2D>();

            // And set its points so that it wraps around the screen
            // Use List<> because we may add another point afterwards
            List<Vector2> points = new List<Vector2> {
                Camera.main.ScreenToWorldPoint(new Vector2(0, 0)),
                Camera.main.ScreenToWorldPoint(new Vector2(0, Camera.main.pixelHeight)),
                Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight)),
                Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, 0))
            };

            // If Add Bottom Edge has been enabled, make it wrap back to the bottom left of
            // the screen so that it completely surrounds it
            if (AddBottomEdge) {
                points.Add(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)));
            }

            m_Collider.points = points.ToArray();
        }
    }
}
