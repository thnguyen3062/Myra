using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceEffectController : MonoBehaviour
{
    [SerializeField] private GameObject startImpact, projecttile,towerImpact,transfer2,cardImpact,endImpact;
    [SerializeField] private GameObject[] transfer1 ;
    // Start is called before the first frame update
    void Start()
    {
        startImpact.SetActive(false);
        projecttile.SetActive(false);
        towerImpact.SetActive(false);
        transfer2.SetActive(false);
        cardImpact.SetActive(false);
        endImpact.SetActive(false);
        foreach(GameObject go in transfer1 )
            go.SetActive(false);
    }
    public IEnumerator OnPierceHit(bool isCardEnd)
    {
        startImpact.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        foreach (GameObject go in transfer1)
            go.SetActive(true);
        projecttile.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        if (!isCardEnd)
        {
            towerImpact.SetActive(true);
            projecttile.SetActive(false);
        }
        else
        {
            transfer2.SetActive(true);
            yield return new WaitForSeconds(0.02f);
            cardImpact.SetActive(true);
            endImpact.SetActive(true);
        }

    }
}
