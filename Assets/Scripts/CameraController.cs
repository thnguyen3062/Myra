using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
	public float designTimeVerticalFieldOfView = 23.5f;
	public int designTimeWidth = 1920;
	public int designTimeHeight = 1080;

	private float hFOVInRads;

	private int prevWidth;
	private int prevHeight;

	void Start()
	{

		prevWidth = designTimeWidth;
		prevHeight = designTimeHeight;

		float aspectRatio = (float)designTimeWidth / (float)designTimeHeight;
		float vFOVInRads = designTimeVerticalFieldOfView * Mathf.Deg2Rad;
		hFOVInRads = 2f * Mathf.Atan(Mathf.Tan(vFOVInRads / 2f) * aspectRatio);

	}

	void Update()
	{

		if (Screen.width != prevWidth || Screen.height != prevHeight)
		{

			float aspectRatio = (float)Screen.width / (float)Screen.height;

			float vFOVInRads = 2f * Mathf.Atan(Mathf.Tan(hFOVInRads / 2f) / aspectRatio);

			foreach (Camera cam in GameObject.FindObjectsOfType(typeof(Camera)))
			{
				cam.fieldOfView = vFOVInRads * Mathf.Rad2Deg;
			}
		}

	}
}
