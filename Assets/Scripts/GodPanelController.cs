using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class GodPanelController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject godContainer;
    [SerializeField] private GameObject godIconContainer;
    [SerializeField] private Transform arrow;
    [SerializeField] private GameObject blockPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnOpen();
    }

    public void OnClose()
    {
        godIconContainer.SetActive(false);
        godContainer.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.1f).onComplete += delegate
        {
            godContainer.SetActive(false);
        };
        blockPanel.gameObject.SetActive(false);
    }

    public void OnOpen()
    {
        godContainer.SetActive(true);
        godContainer.transform.DOScale(Vector3.one, 0.1f).onComplete += delegate
        {
            godIconContainer.SetActive(true);
        };
        blockPanel.gameObject.SetActive(true);
    }
}
