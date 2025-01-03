using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTrackManager : MonoBehaviour
{
    public Transform[] EnemyPaths; // Store all enemy paths

    void Start()
    {
        // Call the function and get the child Transforms for the paths
        EnemyPaths = GetChildTransforms();

        // (Optional) Print the names of the paths to the console
        foreach (Transform path in EnemyPaths)
        {
            Debug.Log("Enemy Path: " + path.name);
        }
    }

    // Function to get all child Transforms of this GameObject
    public Transform[] GetChildTransforms()
    {
        // Get the number of children
        int childCount = transform.childCount;
        Transform[] childTransforms = new Transform[childCount];

        // Loop through each child and add their Transform to the array
        for (int i = 0; i < childCount; i++)
        {
            childTransforms[i] = transform.GetChild(i);
        }

        return childTransforms;
    }
}
