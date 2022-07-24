using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacralLineController : MonoBehaviour
{
    LineRenderer line;
    Transform childPoint;

    private void Start() {
        line = GetComponent<LineRenderer>();
        childPoint = transform.GetChild(0);
    }
    void Update()
    {    
        line.SetPosition(0,  new Vector3(childPoint.position.x, line.GetPosition(0).y,  line.GetPosition(0).z));
        line.SetPosition(1,  new Vector3(childPoint.position.x, line.GetPosition(1).y,  line.GetPosition(1).z));
         
    }
}
