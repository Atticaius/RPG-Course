using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursors : MonoBehaviour {

    CameraRaycaster cameraRaycaster;

    // Layers
    const int walkableLayer = 8;
    const int enemyLayer = 9;

    // Variables
    [SerializeField] Texture2D walkCursor;
    [SerializeField] Texture2D attackCursor;
    [SerializeField] Texture2D unknownCursor;

    Vector2 hotSpot = Vector2.zero;

	// Use this for initialization
	void Start () {
        cameraRaycaster = GetComponent<CameraRaycaster>();
        cameraRaycaster.notifyLayerChangeObservers += OnLayerChange;
	}
	
	// Update is called once per frame
	void OnLayerChange (int newLayer) {
        switch (newLayer)
        {
            case walkableLayer:
                Cursor.SetCursor(walkCursor, hotSpot, CursorMode.Auto);
                break;

            case enemyLayer:
                Cursor.SetCursor(attackCursor, hotSpot, CursorMode.Auto);
                break;

            default:
                Cursor.SetCursor(unknownCursor, hotSpot, CursorMode.Auto);
                break;

        }
	}
}
