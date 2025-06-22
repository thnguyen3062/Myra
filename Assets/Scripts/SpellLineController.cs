using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellLineController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private Texture[] textures;
    private int animationSteps;
    [SerializeField] private float fps = 30;
    private float fpsCounter;

    [SerializeField] private Transform target;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void AssignTarget(Vector3 startPos, Transform newTarget)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        target = newTarget;
    }

    private void Update()
    {
        if (target != null)
        {
            lineRenderer.SetPosition(1, target.position);

            fpsCounter += Time.deltaTime;
            if (fpsCounter >= 1 / fps)
            {
                animationSteps += 1;

                if (animationSteps == textures.Length)
                {
                    animationSteps = 0;
                }

                lineRenderer.material.SetTexture("_MainTex", textures[animationSteps]);
                fpsCounter = 0;
            }
        }
    }
}
