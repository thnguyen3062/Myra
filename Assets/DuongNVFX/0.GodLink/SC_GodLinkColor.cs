using UnityEngine;
using DG.Tweening;

public class SC_GodLinkColor : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Color targetColor = Color.blue;
    public float colorDuration = 2f;
    public float targetSpeed = 2.0f;
    public float speedDuration = 2f;
    public float targetErosionAmount = 0.11f;
    public float targetContrast = 0.11f;
    public float erosionDuration = 2f;
    // public GameObject objectToEnable;
    private Material lineMaterial;
    private bool isColorChanging = false;
    private bool isSpeedChanging = false;
    private bool isErosionChanging = false;
    public GameObject prefabToInstantiate; // Reference to the prefab you want to instantiate.
    private Transform startPoint;
    public Transform endPoint;

    private void Start()
    {
        lineMaterial = lineRenderer.material;
    }
    public void SetPosition(Transform start, Transform end)
    {
        startPoint = start;
        endPoint = end;
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }    
    public void OnGodDead()
    {
        // // Set the LineRenderer positions based on the start and end points.
        // lineRenderer.SetPosition(0, startPoint.position);
        // lineRenderer.SetPosition(1, endPoint.position);

        // Use DOTween to smoothly change the _Color1 property to the targetColor.
        lineMaterial.DOColor(targetColor, "_Color1", colorDuration)
            .OnStart(() => isColorChanging = true)
            .OnComplete(() => isColorChanging = false);

        // Use DOTween to smoothly change the speed property to the targetSpeed.
        DOTween.To(() => lineMaterial.GetFloat("_Speed"), x => lineMaterial.SetFloat("_Speed", x), targetSpeed, speedDuration)
            .OnStart(() => isSpeedChanging = true)
            .OnComplete(() => isSpeedChanging = false);

        // Use DOTween to smoothly change the _ErosionAmount property to the targetErosionAmount.
        DOTween.To(() => lineMaterial.GetFloat("_ErosionAmount"), x => lineMaterial.SetFloat("_ErosionAmount", x), targetErosionAmount, erosionDuration)
            .OnStart(() => isErosionChanging = true)
            .OnComplete(() => isErosionChanging = false);

        // Use DOTween to smoothly change the speed property to the targetSpeed.
        DOTween.To(() => lineMaterial.GetFloat("_Contrast"), x => lineMaterial.SetFloat("_Contrast", x), targetContrast, speedDuration - 0.8f)
            .OnStart(() => isSpeedChanging = true)
            .OnComplete(() => {
                isErosionChanging = false;
                    // Enable the serialized GameObject after the DOTween sequence completes.
                    // Instantiate the prefab instead of enabling the serialized GameObject.
                    if (prefabToInstantiate != null)
                {
                    Instantiate(prefabToInstantiate, endPoint.position, Quaternion.identity);
                }
            });
    }    
    private void Update()
    {
        // Check for a mouse click to trigger the color, speed, and erosion changes.
       
    }
}
