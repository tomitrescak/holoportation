using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.WSA.Input;

public class TapToPlace : MonoBehaviour
{
	bool placing = false;
    bool justDropped = false;
	Quaternion rotOffset;

    public List<GameObject> enables;

	// Called by GazeGestureManager when the user performs a Select gesture
	void OnSelect()
	{
        if (!placing && !justDropped)
        {
            placing = true;
            justDropped = false;
            SpatialMapping.Instance.DrawVisualMeshes = true;

            this.enables.ForEach(o => o.SetActive(true));
        }		
	}

    // Called by GazeGestureManager whenever a tap even it given, regardless of if the object is selected or not
    void OnTap()
    {
        if (placing)
        {
            placing = false;
            justDropped = true;
            SpatialMapping.Instance.DrawVisualMeshes = false;

            this.enables.ForEach(o => o.SetActive(false));

            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }

	// Update is called once per frame
	void Update()
	{
        // If the user is in placing mode,
        // update the placement to match the user's gaze.

        justDropped = false;
        if (placing)
		{
            // Do a raycast into the world that will only hit the Spatial Mapping mesh.
            var headPosition = Camera.main.transform.position;
			var gazeDirection = Camera.main.transform.forward;

			RaycastHit hitInfo;
			if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
				3.0f, SpatialMapping.PhysicsRaycastMask))
			{
                // Move this object's parent object to
                // where the raycast hit the Spatial Mapping mesh.
                Vector3 targetPos = hitInfo.point + (hitInfo.normal * 0.1f);
				transform.position = Vector3.Lerp(transform.position, targetPos, 0.3f);

                // Rotate this object's parent object to face the user.
				transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(-gazeDirection),0.3f);
			}
			else
			{
				transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position + (gazeDirection * 3.0f), 0.3f);
				transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(-gazeDirection), 0.3f);
			}
		}
	}
}