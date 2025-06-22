using UnityEngine;
using UnityEngine.EventSystems;
using GIKCore.Lang;
using GIKCore.Utilities;

public class DragAndDropUI : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerClickHandler
{
    public ICallback.CallFunc onGetShard;
    public ICallback.CallFunc onDragShard;
    public ICallback.CallFunc onEndDragShard;
    public delegate void OnEndDragCallback();
    public event OnEndDragCallback onEndDragCallback;
    private RectTransform rectTransform;

    public void OnDrag(PointerEventData eventData)
    {
        if(GameBattleScene.instance!=null)
        {
            if (GameBattleScene.instance.IsYourTurn)
            {
                if (rectTransform == null)
                {
                    GameObject shardGO = Instantiate(UIManager.instance.shardObject, Input.mousePosition, Quaternion.identity, transform.parent.root/*, UIManager.instance.transform*/);
                    rectTransform = shardGO.GetComponent<RectTransform>();
                    rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
                    rectTransform.position = transform.position;
                    onDragShard?.Invoke();
                }
                Vector3 globalMousePos;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
                {
                    rectTransform.position = new Vector3(globalMousePos.x, globalMousePos.y , globalMousePos.z);
                }
            }
        }
        
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //if(GameBattleScene.instance!= null)
        //{
        //    if (GameBattleScene.instance.IsYourTurn)
        //    {
        //        RaycastHit hit;
        //        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        //        {
        //            if (hit.collider != null)
        //            {
        //                if (hit.collider.GetComponent<BoardCard>() != null)
        //                {
        //                    if (hit.collider.GetComponent<BoardCard>().heroInfo.type == DBHero.TYPE_GOD)
        //                        GameBattleScene.instance.AddShard(hit.collider.GetComponent<BoardCard>());
        //                }
        //            }
        //        }
        //        if (rectTransform != null)
        //            Destroy(rectTransform.gameObject);
        //        onEndDragShard?.Invoke();
        //    }
        //}
        //else
        //{
        //    if(TutorialController.instance.m_TutorialID==1)
        //    {
        //        if (TutorialController.instance.index == 11|| TutorialController.instance.index == 21|| TutorialController.instance.index == 35)
        //        {
        //            RaycastHit hit;
        //            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        //            {
        //                if (hit.collider != null)
        //                {
        //                    if (hit.collider.GetComponent<BoardCard>() != null)
        //                    {
        //                        if (hit.collider.GetComponent<BoardCard>().heroInfo.type == DBHero.TYPE_GOD)
        //                            BattleSceneTutorial.instance.EndDragShard(hit.collider.GetComponent<BoardCard>());
        //                    }
        //                }
        //            }
        //            if (rectTransform != null)
        //                Destroy(rectTransform.gameObject);
        //        }
                
        //    }
        //}
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (rectTransform != null)
        {
            Destroy(rectTransform.gameObject);
            rectTransform = null;


        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameBattleScene.instance == null)
            return;
        if (GameBattleScene.instance.IsYourTurn)
            onGetShard?.Invoke();
    }
}
