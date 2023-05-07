using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classifications : MonoBehaviour
{
    [SerializeField]
    private List<ClassificationData> _classifications;

    public List<ClassificationData> this[int i] => _classifications;
}
