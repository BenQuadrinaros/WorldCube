using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDetector : MonoBehaviour
{
    public LayerMask layerMask;
    private RaycastHit hitInfo;

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.forward, out hitInfo, 15.0f, layerMask))
        {
            Debug.Log("Front face " + hitInfo.transform.parent.name);
        }
    }
}
