using UnityEngine;

public class WorldCursor : MonoBehaviour
{
	private MeshRenderer meshRenderer;

	// Use this for initialization
	void Start()
	{
		// Grab the mesh renderer that's on the same object as this script.
		meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		// Do a raycast into the world based on the user's
		// head position and orientation.
		var headPosition = Camera.main.transform.position;
		var gazeDirection = Camera.main.transform.forward;

		RaycastHit hitInfo;

		if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
		{
			// If the raycast hit a hologram...
			// Display the cursor mesh.
			meshRenderer.enabled = true;

			// Move the cursor to the point where the raycast hit.
			this.transform.position = Vector3.Lerp(transform.position,hitInfo.point,0.5f);

			// Rotate the cursor to hug the surface of the hologram.
			this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, hitInfo.normal), 0.5f);
		}
		else
		{
			// If the raycast did not hit a hologram, hide the cursor mesh.
			meshRenderer.enabled = true;

			this.transform.position =
				Vector3.Lerp(transform.position, Camera.main.transform.position + (-gazeDirection * 5.0f), 0.5f);
			this.transform.rotation = Quaternion.LookRotation(-gazeDirection);
		}
	}
}