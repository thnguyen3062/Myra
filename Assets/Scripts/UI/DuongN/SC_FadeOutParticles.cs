using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_FadeOutParticles : MonoBehaviour
{
    public float fadeDuration = 10f; // Duration of the fade-out effect

    public void StartFadeOut()
    {
        // Lay cacc particle systems trong children
        ParticleSystem[] allParticleSystems = GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem ps in allParticleSystems)
        {
            // Set emission rate to 0
            var emission = ps.emission;
            emission.rateOverTime = 0f;
        }

        // Tat object
        Invoke("DeactivateGameObject", fadeDuration);
    }

    void DeactivateGameObject()
    {
        gameObject.SetActive(false);
    }
}