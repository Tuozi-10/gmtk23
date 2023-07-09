using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [SerializeField] private List<Road> roads;

    [ContextMenu("Get Waypoints")]
    private void GetWaypointsFromRoad()
    {
        roads.Clear();
        
        for (var index = 0; index < transform.childCount; index++)
        {
            var currentRoad = new Road();
            currentRoad.id = index;
            currentRoad.waypointsParent = transform.GetChild(index);

            foreach (var waypoint in currentRoad.waypointsParent.GetComponentsInChildren<Transform>())
            {
                if (waypoint == currentRoad.waypointsParent) continue;
                currentRoad.waypoints.Add(waypoint);
            }
            
            roads.Add(currentRoad);
        }
    }
}

[Serializable]
public class Road
{
    public float id;
    public Transform waypointsParent;
    public List<Transform> waypoints = new List<Transform>();
}
