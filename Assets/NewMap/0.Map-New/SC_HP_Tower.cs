using UnityEngine;

public class SC_HP_Tower : MonoBehaviour
{
    [SerializeField] private float hp_ally;
    [SerializeField] private float hp_enemy;

    [SerializeField] private MeshRenderer ally_mesh;
    [SerializeField] private MeshRenderer enemy_mesh;
    [SerializeField] private ParticleSystem enemy_fire;
    [SerializeField] private ParticleSystem enemy_smoke;
    [SerializeField] private ParticleSystem enemy_smoke_wide;
    [SerializeField] private ParticleSystem ally_fire;
    [SerializeField] private ParticleSystem ally_smoke;
    [SerializeField] private ParticleSystem ally_smoke_wide;
    private float emission_rate_ally = 0;
    private float emission_rate_enemy = 0;
    private ParticleSystem.EmissionModule enemy_fire_var;
    private ParticleSystem.EmissionModule enemy_smoke_var;
    private ParticleSystem.EmissionModule enemy_smoke_wide_var;
    private ParticleSystem.EmissionModule ally_fire_var;
    private ParticleSystem.EmissionModule ally_smoke_var;
    private ParticleSystem.EmissionModule ally_smoke_wide_var;

    private void Start()
    {
        // Get the Particle System's Emission Module.
        enemy_fire_var = enemy_fire.emission;
        enemy_smoke_var = enemy_smoke.emission;
        enemy_smoke_wide_var = enemy_smoke_wide.emission;

        ally_fire_var = ally_fire.emission;
        ally_smoke_var = ally_smoke.emission;
        ally_smoke_wide_var = ally_smoke_wide.emission;
         
        hp_ally = 20;
        hp_enemy = 20;
        UpdateMaterialHp();
    }

    private void Update()
    {
        
    }

    private void UpdateMaterialHp()
    {
            ally_mesh.material.SetFloat("_Hp", hp_ally);
            enemy_mesh.material.SetFloat("_Hp", hp_enemy);
    }
    private void UpdateParticle()
    {
        emission_rate_ally = Mathf.Ceil(10 / (hp_ally + 0.5f));
        emission_rate_enemy = Mathf.Ceil(15 / (hp_enemy + 0.5f));

        // Set the emission rate over time.
        enemy_fire_var.rateOverTime = emission_rate_enemy;
        enemy_smoke_var.rateOverTime = emission_rate_enemy;
        enemy_smoke_wide_var.rateOverTime = emission_rate_enemy;

        ally_fire_var.rateOverTime = emission_rate_ally + 1f;
        ally_smoke_var.rateOverTime = emission_rate_ally;
        ally_smoke_wide_var.rateOverTime = emission_rate_ally;
    }
    public void UpdateTowerState(float hp, bool isMe )
    {
        if(isMe)
        {
            hp_ally = (float) hp/10;
            UpdateMaterialHp();
            UpdateParticle();

        }    
        else
        {
            hp_enemy =(float) hp/10;
            UpdateMaterialHp();
            UpdateParticle();
        }
        
    }
}