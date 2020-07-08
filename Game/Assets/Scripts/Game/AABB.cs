using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB : MonoBehaviour
{
    public Bounds BoundBox;

    public Color GizmoColor = Color.white;

    void Awake()
    {
        BoundBox.center = transform.position;
    }

    void OnDrawGizmos()
    {
        GizmoColor.a = 0.3f;
        Gizmos.color = GizmoColor;
        Gizmos.DrawCube(BoundBox.center, BoundBox.size);
    }
}
