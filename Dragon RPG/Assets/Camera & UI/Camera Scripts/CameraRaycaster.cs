using UnityEngine;

public class CameraRaycaster : MonoBehaviour
{
    public Layer[] layerPriorities = {
        Layer.Enemy,
        Layer.Walkable
    };

    float distanceToBackground = 100f;
    Camera viewCamera;

    RaycastHit rayCastHit;
    public RaycastHit Hit
    {
        get { return rayCastHit; }
    }

    Layer layerHit;
    public Layer CurrentLayerHit
    {
        get { return layerHit; }
    }

    public delegate void LayerChange (Layer newLayer);
    public event LayerChange layerChangeObservers;

    void Start()
    {
        viewCamera = Camera.main;
    }

    void Update()
    {
        // Look for and return priority layer hit
        foreach (Layer layer in layerPriorities)
        {
            var hit = RaycastForLayer(layer);
            if (hit.HasValue)
            {
                rayCastHit = hit.Value;
                if (layerHit != layer)
                {
                    layerHit = layer;
                    layerChangeObservers(layerHit);
                }
                layerHit = layer;
                return;
            }
        }

        // Otherwise return background hit
        rayCastHit.distance = distanceToBackground;
        if (layerHit != Layer.RaycastEndStop)
        {
            layerHit = Layer.RaycastEndStop;
            layerChangeObservers(layerHit);
        }
    }

    RaycastHit? RaycastForLayer(Layer layer)
    {
        int layerMask = 1 << (int)layer; // See Unity docs for mask formation
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit; // used as an out parameter
        bool hasHit = Physics.Raycast(ray, out hit, distanceToBackground, layerMask);
        if (hasHit)
        {
            return hit;
        }
        return null;
    }
}
