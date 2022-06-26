using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer line;
    [HideInInspector] public Transform Point1, Point2;
    
    private void OnEnable() {
        line = GetComponent<LineRenderer>();
        Point1 = transform.GetChild(0);
        Point2 = transform.GetChild(1);
    }

    void Update()
    {
        line.SetPosition(0, Point1.position);
        line.SetPosition(1, Point2.position);
    }
}
