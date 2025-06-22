using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask groundLayer;

    /// <summary>
    /// How many points will be used for height curve?
    /// </summary>
    [Range(2, 32)]
    [SerializeField] private ushort lineRendererThickness = 16;

    [SerializeField] private AnimationCurve heightCurve;

    [SerializeField] private float gamePadSensivity = 0.01f;
    private Vector3[] points;

    /// <summary>
    /// arrow at the last point with direction.
    /// </summary>
    [SerializeField] private Transform apex;

    private Vector3 currentScreenPosition;
    private Vector3 startingPosition;

    private Card currentCard;
    public Transform cardClone, godUIClone;

    private void Awake()
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = lineRendererThickness;
        points = new Vector3[lineRendererThickness];
    }

    public void InitCursor(Vector3 startPosition, Card card, Transform clone, Transform cloneGodUI = null)
    {
        startingPosition = startPosition;
        currentCard = card;
        cardClone = clone;
        godUIClone = cloneGodUI;
    }
    private void OnDisable()
    {
        currentCard = null;
        cardClone = null;
        godUIClone = null;
    }
    private void Update()
    {
        if (GameBattleScene.instance != null)
        {
            if (!GameBattleScene.instance.IsYourTurn)
                return;
            currentScreenPosition = Input.mousePosition;
            currentScreenPosition.x = Mathf.Clamp(currentScreenPosition.x, 0, Screen.width);
            currentScreenPosition.y = Mathf.Clamp(currentScreenPosition.y, 0, Screen.height);

            var ray = Camera.main.ScreenPointToRay(currentScreenPosition);
            RaycastHit hit;
            if (currentCard != null)
            {
                if (currentCard.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || currentCard.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                {
                    if (Physics.Raycast(ray, out hit))
                    {
                        Vector3 hitPosition = hit.point;
                        Set(hitPosition);
                        cardClone.transform.position = GameBattleScene.instance.magicSpawnPosition.position;
                    }
                }
                else
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
                    {
                        Vector3 hitPosition = hit.point;
                        hitPosition.y = 0;
                        Set(hitPosition);
                        float distance = Mathf.Clamp(hitPosition.x - cardClone.position.x, -1, 1);
                        cardClone.GetComponent<CardOnBoardClone>().SetPosition(hitPosition, distance);
                        if (hit.collider.GetComponent<CardSlot>() != null)
                        {
                            if (hit.collider.GetComponent<CardSlot>().type == SlotType.Player)
                            {
                                cardClone.transform.position = new Vector3(hit.collider.transform.position.x, 0, hit.collider.transform.position.z);
                                cardClone.GetComponent<CardOnBoardClone>().cloneSlot = hit.collider.GetComponent<CardSlot>();
                                cardClone.gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }
            else
            {
                //la god ui 
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
                {
                    Vector3 hitPosition = hit.point;
                    Set(hitPosition);
                    float distance = Mathf.Clamp(hitPosition.x - cardClone.position.x, -1, 1);
                    cardClone.GetComponent<CardOnBoardClone>().SetPosition(hitPosition, distance);
                    //godUIClone.position = Vector3.Lerp(transform.position, hitPosition, 6 * Time.deltaTime);
                    if (hit.collider.name != "Plane")
                    {
                        cardClone.gameObject.SetActive(true);
                        godUIClone.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            if (!BattleSceneTutorial.instance.IsYourTurn)
                return;
            currentScreenPosition = Input.mousePosition;
            currentScreenPosition.x = Mathf.Clamp(currentScreenPosition.x, 0, Screen.width);
            currentScreenPosition.y = Mathf.Clamp(currentScreenPosition.y, 0, Screen.height);

            var ray = Camera.main.ScreenPointToRay(currentScreenPosition);
            RaycastHit hit;
            if (currentCard != null)
            {
                if (currentCard.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || currentCard.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                {
                    if (Physics.Raycast(ray, out hit))
                    {
                        Vector3 hitPosition = hit.point;
                        Set(hitPosition);
                        cardClone.transform.position = BattleSceneTutorial.instance.magicSpawnPosition.position;
                    }
                }
                else
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
                    {
                        Vector3 hitPosition = hit.point;
                        hitPosition.y = 0;
                        Set(hitPosition);
                        float distance = Mathf.Clamp(hitPosition.x - cardClone.position.x, -1, 1);
                        cardClone.GetComponent<CardOnBoardClone>().SetPosition(hitPosition, distance);
                        if (hit.collider.GetComponent<CardSlot>() != null)
                        {
                            if (hit.collider.GetComponent<CardSlot>().type == SlotType.Player)
                            {
                                cardClone.transform.position = new Vector3(hit.collider.transform.position.x, 0, hit.collider.transform.position.z);
                                cardClone.GetComponent<CardOnBoardClone>().cloneSlot = hit.collider.GetComponent<CardSlot>();
                                cardClone.gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }
            else
            {
                //la god ui 
                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 hitPosition = hit.point;
                    Set(hitPosition);
                    float distance = Mathf.Clamp(hitPosition.x - cardClone.position.x, -1, 1);
                    cardClone.GetComponent<CardOnBoardClone>().SetPosition(hitPosition, distance);
                    //godUIClone.position = Vector3.Lerp(transform.position, hitPosition, 6 * Time.deltaTime);
                    if (hit.collider.name != "Plane")
                    {
                        cardClone.gameObject.SetActive(true);
                        godUIClone.gameObject.SetActive(false);
                    }
                }
            }
        }

    }

    private void Set(Vector3 endPosition)
    {
        if (lineRenderer.positionCount != lineRendererThickness)
            lineRenderer.positionCount = lineRendererThickness;

        points[0] = startingPosition;
        points[0].y = 0.3f;

        Vector3 dir = endPosition - startingPosition;
        float dist = Mathf.Clamp(Vector3.Distance(startingPosition, endPosition), 0, Vector3.Distance(startingPosition, endPosition) - 0.9f);
        Vector3 endTmp = startingPosition + (dir.normalized * dist);

        points[lineRendererThickness - 1] = endTmp;
        points[lineRendererThickness - 1].y = 0.3f;

        lineRenderer.SetPositions(points);

        apex.position = endPosition;
        apex.rotation = Quaternion.LookRotation(endPosition - points[lineRendererThickness - 2]);
    }
}
