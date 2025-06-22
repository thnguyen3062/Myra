using GIKCore.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnNumberHandle : MonoBehaviour
{
    private MeshRenderer mesh;
    private float currentCount = -1;
    private float targetCount = -1;
    private float countSpeed = 0.1f;

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        GameBattleScene.instance.onGameBattleChangeTurn += OnGameStartBattle;
        GameBattleScene.instance.onResumeRoundCount += OnResume;
    }

    private void Update()
    {
        if(targetCount > currentCount)
        {
            currentCount += countSpeed;
            if (currentCount >= targetCount)
            {
                currentCount = targetCount;
            }
        }
        else if(targetCount < currentCount)
        {
            currentCount -= countSpeed;
            if(currentCount <= targetCount)
            {
                currentCount = targetCount;
            }
        }
        mesh.material.SetFloat("_Rotate", currentCount);
        if (currentCount <= 0)
        {
            mesh.material.SetFloat("_Turn", currentCount + 1);
            mesh.material.SetFloat("_Combat", 0);
        }
        else
            mesh.material.SetFloat("_Combat", 1);
    }

    private void OnGameStartBattle(long index)
    {
        targetCount = index;
        SoundHandler.main.PlaySFX("ChangePhase", "sounds");
    }

    private void OnResume(long index)
    {
        targetCount = index;
        SoundHandler.main.PlaySFX("ChangePhase", "sounds");
    }
}
