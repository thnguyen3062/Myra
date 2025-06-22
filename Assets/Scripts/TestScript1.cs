using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript1 : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField]
    private LayerMask groundLayer;

    /// <summary>
    /// How many points will be used for height curve?
    /// </summary>
    [Range(2, 32)]
    [SerializeField] private ushort lineRendererThickness = 16;

    [SerializeField] private AnimationCurve heightCurve;

    [SerializeField] private float gamePadSensivity = 0.01f;
    private Vector3[] points;

    /// <summary>
    /// arrow at the last point with direction.
    /// </summary>
    [SerializeField] private Transform apex;

    private Vector3 currentScreenPosition;
    private Vector3 startingPosition;

    private void Awake()
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = lineRendererThickness;
        points = new Vector3[lineRendererThickness];
    }

    public void InitCursor(Vector3 startPosition)
    {
        startingPosition = startPosition;
    }

    public void SetCursorPosition(Vector3 mousePos)
    {
        currentScreenPosition = mousePos;
        currentScreenPosition.x = Mathf.Clamp(currentScreenPosition.x, 0, Screen.width);
        currentScreenPosition.y = Mathf.Clamp(currentScreenPosition.y, 0, Screen.height);

        var ray = Camera.main.ScreenPointToRay(currentScreenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 hitPosition = hit.point;
            Set(hitPosition);
        }
    }

    private void Set(Vector3 endPosition)
    {
        if (lineRenderer.positionCount != lineRendererThickness)
            lineRenderer.positionCount = lineRendererThickness;

        points[0] = startingPosition;
        points[0].y = 0.3f;

        Vector3 dir = endPosition - startingPosition;
        float dist = Mathf.Clamp(Vector3.Distance(startingPosition, endPosition), 0, Vector3.Distance(startingPosition, endPosition) - 0.9f);
        Vector3 endTmp = startingPosition + (dir.normalized * dist);

        points[lineRendererThickness - 1] = endTmp;
        points[lineRendererThickness - 1].y = 0.3f;

        lineRenderer.SetPositions(points);

        apex.position = endPosition;
        apex.rotation = Quaternion.LookRotation(endPosition - points[lineRendererThickness - 2]);
    }
}