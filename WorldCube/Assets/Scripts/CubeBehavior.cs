using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehavior : MonoBehaviour
{
    public static CubeBehavior Instance { get; private set; }

    Quaternion cubeRotation;
    Vector3 cubePosition;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = cubePosition;
        gameObject.transform.rotation = cubeRotation;
    }

    // Accepts values from GyroscopeCommunication script on call
    public void setCubePosition(float[] values)
    {
        // For now, we should only get rotation

        //cubePosition.x = values[0];
        //cubePosition.y = values[1];
        //cubePosition.z = values[2];
    }

    // Accepts values from GyroscopeCommunication script on call
    public void setCubeRotation(float[] rotations)
    {
        cubeRotation = new Quaternion(rotations[0], rotations[1], rotations[2], cubeRotation.w);
    }
}
