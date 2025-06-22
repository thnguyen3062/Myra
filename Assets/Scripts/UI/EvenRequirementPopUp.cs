using GIKCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvenRequirementPopUp : MonoBehaviour
{
    public void DoClickClose()
    {
        this.gameObject.SetActive(false);
    }
    public void DoClickQuestion()
    {

    }
    public void DoClickEnter()
    {
        Game.main.socket.GetUserDeck();
        Game.main.LoadScene("SelectDeckEventScene", delay: 0.3f, curtain: true);
    }

}
