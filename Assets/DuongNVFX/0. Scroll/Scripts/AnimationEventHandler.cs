using UnityEngine;
using System.Collections;

public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] private RectTransform parentOpp;
    [SerializeField] private RectTransform parentYou;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private float destructionDelay = 2.5f;

    public void DestroyGem_Opp()
    {
        InstantiateAndDestroyParticle(particlePrefab, parentOpp);
    }

    public void DestroyGem_You()
    {
        InstantiateAndDestroyParticle(particlePrefab, parentYou);
    }

    private void InstantiateAndDestroyParticle(GameObject particlePrefab, RectTransform parent)
    {
        if (particlePrefab != null && parent != null)
        {
            parent.gameObject.SetActive(true); // Turn on parent

            // Instantiate
            GameObject instantiatedParticle = Instantiate(particlePrefab, parent.position, parent.rotation, parent);

            // Turn on particle
            instantiatedParticle.SetActive(true);

            // Coroutine to destroy the particle
            StartCoroutine(DestroyParticle(instantiatedParticle, parent));
        }
        else
        {
            Debug.LogWarning("No Particle prefab or parent assigned.");
        }
    }

    private IEnumerator DestroyParticle(GameObject particle, Transform parent)
    {
        // Wait for delay time
        yield return new WaitForSeconds(destructionDelay);

        // Destroy the particle
        Destroy(particle);

        // Turn off parent
        parent.gameObject.SetActive(false);
    }
}
