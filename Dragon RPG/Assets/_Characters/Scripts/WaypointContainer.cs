using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class WaypointContainer : MonoBehaviour
    {
        private void OnDrawGizmos ()
        {
            Vector3 firstPosition = transform.GetChild(0).position;
            Vector3 previousPosition = firstPosition;

            foreach (Transform waypointTransform in transform)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(waypointTransform.position, .2f);
                Gizmos.color = Color.black;
                Gizmos.DrawLine(waypointTransform.position, previousPosition);
                previousPosition = waypointTransform.position;
            }
            Gizmos.DrawLine(previousPosition, firstPosition);
        }
    }
}