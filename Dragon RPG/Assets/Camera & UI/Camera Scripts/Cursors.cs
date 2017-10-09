using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursors : MonoBehaviour {

    CameraRaycaster cameraRaycaster;

    // Variables
    [SerializeField] Texture2D walkCursor;
    [SerializeField] Texture2D attackCursor;
    [SerializeField] Texture2D unknownCursor;

    Vector2 hotSpot = Vector2.zero;

	// Use this for initialization
	void Start () {
        cameraRaycaster = GetComponent<CameraRaycaster>();	
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(cameraRaycaster.layerHit);
        switch (cameraRaycaster.layerHit)
        {
            case Layer.Walkable:
                Cursor.SetCursor(walkCursor, hotSpot, CursorMode.Auto);
                break;
            case Layer.Enemy:
                Cursor.SetCursor(attackCursor, hotSpot, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(unknownCursor, hotSpot, CursorMode.Auto);
                break;

        }
	}
}
