using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFXChangeSprite : MonoBehaviour
{
    [SerializeField] private Material m_GodImortalMaterial;
    [SerializeField] private Material m_GodLegendMaterial;
    [SerializeField] private Material m_GodLowMaterial;
    [SerializeField] private Material m_MortalHighMaterial;
    [SerializeField] private Material m_MortalLowMaterial;
    private ParticleSystemRenderer m_Renderer;

    public void SetAttack(bool isGod, long rarity)
    {
        if (m_Renderer == null)
            m_Renderer = GetComponent<ParticleSystemRenderer>();

        if (isGod)
        {
            if (rarity == 5)
                m_Renderer.material = m_GodImortalMaterial;
            else if (rarity == 4)
                m_Renderer.material = m_GodLegendMaterial;
            else
                m_Renderer.material = m_GodLowMaterial;
        }
        else
        {
            if (rarity <= 3)
                m_Renderer.material = m_MortalLowMaterial;
            else
                m_Renderer.material = m_MortalHighMaterial;
        }
    }
}
