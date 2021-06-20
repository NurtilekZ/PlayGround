using UnityEngine;

public class DebugDrawLine : MonoBehaviour
{
    public Transform from;
    public Transform to;
    private void OnDrawGizmos()
    {
        Debug.DrawLine(from.position, to.position, Color.green);
    }
}
