using UnityEngine;
using System.Collections.Generic;

public class WaypointNode : MonoBehaviour
{
    [Tooltip("The next possible waypoints the AI can travel to from this one.")]
    public List<WaypointNode> nextWaypoints;
}