using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using RPG.Characters;

namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour
    {
        const float MAX_RAYCAST_DEPTH = 100f; // Max depth that the raycaster can hit
        const int POTENIALLY_WALKABLE_LAYER = 8;

        // Cursors
        [SerializeField] Texture2D walkCursor;
        [SerializeField] Texture2D enemyCursor;
        [SerializeField] Texture2D unknownCursor;
        [SerializeField] Vector2 hotSpot = Vector2.zero;

        

        // Setup delegates for broadcasting layer changes to other classes
        public delegate void OnMouseOverTerrain (Vector3 Destination);
        public event OnMouseOverTerrain notifyMouseOverPotentiallyWalkable;

        public delegate void OnMouseOverEnemy (Enemy enemy);
        public event OnMouseOverEnemy notifyMouseOverEnemy;

        void LateUpdate ()
        {
            // Check if pointer is over an interactable UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Implement UI interaction
            }
            else
            {
                PerformRaycasts();
            }
        }

        void PerformRaycasts ()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (RaycastForEnemy(ray))
            {
                return;
            }
            if (RaycastForPotentiallyWalkable(ray))
            {
                return;
            }
        }

        bool RaycastForEnemy (Ray ray)
        {
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, MAX_RAYCAST_DEPTH);
            if (hitInfo.collider == null)
            {
                return false;
            } else if (hitInfo.collider.gameObject.GetComponent<Enemy>())
            {
                GameObject gameObjectHit = hitInfo.collider.gameObject;
                Cursor.SetCursor(enemyCursor, hotSpot, CursorMode.Auto);
                Enemy enemy = gameObjectHit.GetComponent<Enemy>();
                notifyMouseOverEnemy(enemy);
                return true;
            }
            return false;
        }

        bool RaycastForPotentiallyWalkable (Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask potentiallyWalkableLayer = 1 << POTENIALLY_WALKABLE_LAYER;
            bool potentiallyWalkableHit = Physics.Raycast(ray, out hitInfo, potentiallyWalkableLayer);
            if (potentiallyWalkableHit)
            {
                Cursor.SetCursor(walkCursor, hotSpot, CursorMode.Auto);
                notifyMouseOverPotentiallyWalkable(hitInfo.point);
                return true;
            }
            return false;
        }
    }
}