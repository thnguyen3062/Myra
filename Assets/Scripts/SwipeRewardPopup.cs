using DG.Tweening;
using GIKCore.UI;
using GIKCore.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SwipeRewardPopup : MonoBehaviour
{
    // Fields
    //[SerializeField] GameObject leftBut, rightBut;
    [SerializeField][Range(1, 10)] int m_TransitionSpeed = 5;
    [SerializeField][Range(0, 2)] float m_NeibourReductionPercentage = 0.8f;
    [SerializeField][Range(1, 2)] float m_MainScalePercentage = 1f;
    [SerializeField] bool m_ScrollWhenReleased = true;
    [SerializeField][Range(0.05f, 1)] float m_ScrollStopSpeed = 0.1f;
    [SerializeField] Scrollbar m_ScrollBar;
    [SerializeField] HorizontalLayoutGroup m_Layout;
    [SerializeField] bool m_HasPadding = true;
    [SerializeField] GoOnOff m_Knob;
    [SerializeField] GameObject m_KnobContainer;
    [SerializeField] GameObject m_LeftSwipe, m_RightSwipe;
    // Values
    //public Color[] colors;
    //public GameObject scrollbar/*, imageContent*/;
    //public float scroll_pos = 0;
    //float[] pos;
    //private bool runIt = false;
    //private float time;
    //private Button takeTheBtn;
    //int btnNumber;

    private Vector2 neighbourScale;
    private Vector2 mainScale;
    private float scrollbarValue = 0;
    private float[] attractionPoints;
    private float subdivisionDistance;
    private float attractionPoint;
    private int childCount;
    private int childWidth = 413; // width of child
    private int mainChildIndex = 0; // always start from 0
    private List<GoOnOff> knobs = new List<GoOnOff>();
    private bool hasKnob = false;

    public event ICallback.CallFunc2<int> onSwipeStop = null;
    public void OnSwipeStop(int index)
    {
        onSwipeStop?.Invoke(index);
    }
    public SwipeRewardPopup SetOnSwipeStopCallback(ICallback.CallFunc2<int> func)
    {
        onSwipeStop = func;
        return this;
    }



    //public ButtonClickEvent SetOnClickCallback(ICallback.CallFunc func) { onClickCB = func; return this; }

    // Methods
    public void SwipeLeft()
    {
        m_Layout.GetComponent<RectTransform>().DOAnchorPosX(-1360f * (mainChildIndex - 1), 0.5f).SetEase(Ease.InOutQuad);
    }
    public void SwipeRight()
    {
        m_Layout.GetComponent<RectTransform>().DOAnchorPosX(-1360f * (mainChildIndex + 1), 0.5f).SetEase(Ease.InOutQuad);
    }
    private void OnEnable()
    {
        m_Layout.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        mainChildIndex = 0; // reset
        if (m_Knob != null)
        {
            hasKnob = true;
        }
        if (m_HasPadding)
        {
            m_Layout.padding.left = (Screen.width - childWidth) / 2;
            m_Layout.padding.right = (Screen.width - childWidth) / 2;
        }
        childCount = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.active)
                childCount++;
        }
        attractionPoints = new float[childCount];
        //childCount = attractionPoints.Length;
        subdivisionDistance = 1f / (childCount - 1f);

        // remove all child knob
        if (hasKnob)
        {
            knobs.Clear();
            for (int i = 0; i < m_KnobContainer.transform.childCount; i++)
            {
                GameObject.Destroy(m_KnobContainer.transform.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < childCount; i++)
        {
            attractionPoints[i] = subdivisionDistance * i;
            if (hasKnob)
            {
                GameObject go = Instantiate(m_Knob.gameObject, m_KnobContainer.transform);
                go.SetActive(true);
                go.GetComponent<GoOnOff>().Turn(i == 0);
                knobs.Add(go.GetComponent<GoOnOff>());
            }
        }

        foreach (Transform child in transform)
        {
            child.localScale = new Vector2(m_NeibourReductionPercentage, m_NeibourReductionPercentage);
        }
        if (childCount > 0)
        {
            transform.GetChild(0).localScale = Vector2.one;
        }
        if (m_LeftSwipe != null)
            m_LeftSwipe.SetActive(childCount > 1);

        if (m_RightSwipe != null)
            m_RightSwipe.SetActive(childCount > 1);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) || (m_ScrollWhenReleased && GetScrollSpeed() > m_ScrollStopSpeed))
        {
            scrollbarValue = m_ScrollBar.value;
            FindAttractionPoint();
            UpdateUI();
        }
        else if (IsBeingScaled())
        {
            m_ScrollBar.value = Mathf.Lerp(m_ScrollBar.value, attractionPoint, m_TransitionSpeed * Time.deltaTime);
            UpdateUI();
        }
        else
        {
            if (onSwipeStop != null)
                onSwipeStop(mainChildIndex); // get main child index when done

            if (hasKnob)
            {
                foreach (GoOnOff goo in knobs)
                {
                    goo.TurnOff();
                }
                knobs[mainChildIndex].TurnOn();
            }
            if (m_LeftSwipe != null)
                m_LeftSwipe.SetActive(mainChildIndex != 0);
            if (m_RightSwipe != null)
                m_RightSwipe.SetActive(mainChildIndex != (childCount - 1));
        }
    }
    private void FindAttractionPoint()
    {
        if (scrollbarValue < 0)
            attractionPoint = 0;
        else
        {
            for (int i = 0; i < childCount; i++)
            {
                if (scrollbarValue < attractionPoints[i] + (subdivisionDistance / 2) && scrollbarValue > attractionPoints[i] - (subdivisionDistance / 2))
                {
                    attractionPoint = attractionPoints[i];
                    mainChildIndex = i;
                }
                //if (i == childCount - 1)
                //{
                //    attractionPoint = attractionPoints[i];
                //}
            }
        }
    }
    private void UpdateUI()
    {
        for (int i = 0; i < attractionPoints.Length; i++)
        {
            if (attractionPoints[i] == attractionPoint)
            {
                //m_KnobContainer.GetChild(i).GetComponent<Image>().sprite = m_Knobs[0];
                mainScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(m_MainScalePercentage, m_MainScalePercentage), 20 * m_TransitionSpeed * Time.deltaTime);
                transform.GetChild(i).localScale = mainScale;
            }
            else
            {
                //m_KnobContainer.GetChild(i).GetComponent<Image>().sprite = m_Knobs[1];
                neighbourScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(m_NeibourReductionPercentage, m_NeibourReductionPercentage), 20 * m_TransitionSpeed * Time.deltaTime);
                transform.GetChild(i).localScale = neighbourScale;
            }
        }
    }
    private float GetScrollSpeed()
    {
        return Mathf.Abs(scrollbarValue - m_ScrollBar.value) / Time.deltaTime;
    }
    private bool IsBeingScaled()
    {
        return Mathf.Abs(m_ScrollBar.value - attractionPoint) > 0.001f || mainScale.x < 0.999f || neighbourScale.x > m_NeibourReductionPercentage + 0.001f;
    }
}
