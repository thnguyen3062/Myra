﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using GIKCore.Lang;


#pragma warning disable 0618 // Disabled warning due to SetVertices being deprecated until new release with SetMesh() is available.

namespace TMPro.Examples
{

    public class TMP_TextSelector_B : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler
    {
        public RectTransform TextPopup_Prefab_01;
        private RectTransform m_TextPopup_RectTransform;
        private TextMeshProUGUI m_TextPopup_TMPComponent;
        private const string k_LinkText = "You have selected link <#ffff00>";
        private const string k_WordText = "Word Index: <#ffff00>";
        private TextMeshProUGUI m_TextMeshPro;
        private Canvas m_Canvas;
        private Camera m_Camera;

        // Flags
        private bool isHoveringObject;
        private int m_selectedWord = -1;
        private int m_selectedLink = -1;
        private int m_lastIndex = -1;

        private Matrix4x4 m_matrix;

        private TMP_MeshInfo[] m_cachedMeshInfoVertexData;

        void Awake()
        {
            m_TextMeshPro = gameObject.GetComponent<TextMeshProUGUI>();


            m_Canvas = gameObject.GetComponentInParent<Canvas>();

            // Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
            if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                m_Camera = null;
            else
                m_Camera = m_Canvas.worldCamera;

            // Create pop-up text object which is used to show the link information.
            m_TextPopup_RectTransform = Instantiate(TextPopup_Prefab_01) as RectTransform;
            m_TextPopup_RectTransform.SetParent(m_Canvas.transform, false);
            m_TextPopup_TMPComponent = m_TextPopup_RectTransform.GetComponentInChildren<TextMeshProUGUI>();
            m_TextPopup_RectTransform.gameObject.SetActive(false);
        }


        void OnEnable()
        {
            // Subscribe to event fired when text object has been regenerated.
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
        }

        void OnDisable()
        {
            // UnSubscribe to event fired when text object has been regenerated.
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
        }


        void ON_TEXT_CHANGED(Object obj)
        {
            if (obj == m_TextMeshPro)
            {
                // Update cached vertex data.
                m_cachedMeshInfoVertexData = m_TextMeshPro.textInfo.CopyMeshInfoVertexData();
            }
        }


        void LateUpdate()
        {
            if (isHoveringObject)
            {
                #region Example of Link Handling
                // Check if mouse intersects with any links.
                int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, Input.mousePosition, m_Camera);
                // Clear previous link selection if one existed.
                if ((linkIndex == -1 && m_selectedLink != -1) || linkIndex != m_selectedLink)
                {
                    m_TextPopup_RectTransform.gameObject.SetActive(false);
                    m_selectedLink = -1;
                }
                // Handle new Link selection.
                if (linkIndex != -1 && linkIndex != m_selectedLink)
                {
                    m_selectedLink = linkIndex;
                    TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];
                    m_TextPopup_RectTransform.gameObject.SetActive(true);
                    m_TextPopup_TMPComponent.text = LangHandler.Get(linkInfo.GetLinkID());
                }

                if (m_TextPopup_RectTransform.gameObject.activeSelf)
                {
                    Vector3 worldPointInRectangle;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(m_TextMeshPro.rectTransform, Input.mousePosition, m_Camera, out worldPointInRectangle);
                    m_TextPopup_RectTransform.position = worldPointInRectangle;
                }
                #endregion

            }
            else
            {
                // Restore any character that may have been modified
                if (m_lastIndex != -1)
                {
                    RestoreCachedVertexAttributes(m_lastIndex);
                    m_lastIndex = -1;
                }
                if (m_TextPopup_RectTransform.gameObject.activeSelf)
                {
                    m_TextPopup_RectTransform.gameObject.SetActive(false);
                    m_selectedLink = -1;
                }
            }

        }

        


        public void OnPointerEnter(PointerEventData eventData)
        {
            isHoveringObject = true;
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            isHoveringObject = false;
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            //Debug.Log("Click at POS: " + eventData.position + "  World POS: " + eventData.worldPosition);

            // Check if Mouse Intersects any of the characters. If so, assign a random color.
            #region Character Selection Handling
            /*
            int charIndex = TMP_TextUtilities.FindIntersectingCharacter(m_TextMeshPro, Input.mousePosition, m_Camera, true);
            if (charIndex != -1 && charIndex != m_lastIndex)
            {
                //Debug.Log("Character [" + m_TextMeshPro.textInfo.characterInfo[index].character + "] was selected at POS: " + eventData.position);
                m_lastIndex = charIndex;

                Color32 c = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
                int vertexIndex = m_TextMeshPro.textInfo.characterInfo[charIndex].vertexIndex;

                UIVertex[] uiVertices = m_TextMeshPro.textInfo.meshInfo.uiVertices;

                uiVertices[vertexIndex + 0].color = c;
                uiVertices[vertexIndex + 1].color = c;
                uiVertices[vertexIndex + 2].color = c;
                uiVertices[vertexIndex + 3].color = c;

                m_TextMeshPro.canvasRenderer.SetVertices(uiVertices, uiVertices.Length);
            }
            */
            #endregion


            #region Word Selection Handling
            //Check if Mouse intersects any words and if so assign a random color to that word.
            /*
            int wordIndex = TMP_TextUtilities.FindIntersectingWord(m_TextMeshPro, Input.mousePosition, m_Camera);

            // Clear previous word selection.
            if (m_TextPopup_RectTransform != null && m_selectedWord != -1 && (wordIndex == -1 || wordIndex != m_selectedWord))
            {
                TMP_WordInfo wInfo = m_TextMeshPro.textInfo.wordInfo[m_selectedWord];

                // Get a reference to the uiVertices array.
                UIVertex[] uiVertices = m_TextMeshPro.textInfo.meshInfo.uiVertices;

                // Iterate through each of the characters of the word.
                for (int i = 0; i < wInfo.characterCount; i++)
                {
                    int vertexIndex = m_TextMeshPro.textInfo.characterInfo[wInfo.firstCharacterIndex + i].vertexIndex;

                    Color32 c = uiVertices[vertexIndex + 0].color.Tint(1.33333f);

                    uiVertices[vertexIndex + 0].color = c;
                    uiVertices[vertexIndex + 1].color = c;
                    uiVertices[vertexIndex + 2].color = c;
                    uiVertices[vertexIndex + 3].color = c;
                }

                m_TextMeshPro.canvasRenderer.SetVertices(uiVertices, uiVertices.Length);

                m_selectedWord = -1;
            }

            // Handle word selection
            if (wordIndex != -1 && wordIndex != m_selectedWord)
            {
                m_selectedWord = wordIndex;

                TMP_WordInfo wInfo = m_TextMeshPro.textInfo.wordInfo[wordIndex];

                // Get a reference to the uiVertices array.
                UIVertex[] uiVertices = m_TextMeshPro.textInfo.meshInfo.uiVertices;

                // Iterate through each of the characters of the word.
                for (int i = 0; i < wInfo.characterCount; i++)
                {
                    int vertexIndex = m_TextMeshPro.textInfo.characterInfo[wInfo.firstCharacterIndex + i].vertexIndex;

                    Color32 c = uiVertices[vertexIndex + 0].color.Tint(0.75f);

                    uiVertices[vertexIndex + 0].color = c;
                    uiVertices[vertexIndex + 1].color = c;
                    uiVertices[vertexIndex + 2].color = c;
                    uiVertices[vertexIndex + 3].color = c;
                }

                m_TextMeshPro.canvasRenderer.SetVertices(uiVertices, uiVertices.Length);
            }
            */
            #endregion


            #region Link Selection Handling
            /*
            // Check if Mouse intersects any words and if so assign a random color to that word.
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, Input.mousePosition, m_Camera);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];
                int linkHashCode = linkInfo.hashCode;

                //Debug.Log(TMP_TextUtilities.GetSimpleHashCode("id_02"));

                switch (linkHashCode)
                {
                    case 291445: // id_01
                        if (m_LinkObject01 == null)
                            m_LinkObject01 = Instantiate(Link_01_Prefab);
                        else
                        {
                            m_LinkObject01.gameObject.SetActive(true);
                        }

                        break;
                    case 291446: // id_02
                        break;

                }

                // Example of how to modify vertex attributes like colors
                #region Vertex Attribute Modification Example
                UIVertex[] uiVertices = m_TextMeshPro.textInfo.meshInfo.uiVertices;

                Color32 c = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
                for (int i = 0; i < linkInfo.characterCount; i++)
                {
                    TMP_CharacterInfo cInfo = m_TextMeshPro.textInfo.characterInfo[linkInfo.firstCharacterIndex + i];

                    if (!cInfo.isVisible) continue; // Skip invisible characters.

                    int vertexIndex = cInfo.vertexIndex;

                    uiVertices[vertexIndex + 0].color = c;
                    uiVertices[vertexIndex + 1].color = c;
                    uiVertices[vertexIndex + 2].color = c;
                    uiVertices[vertexIndex + 3].color = c;
                }

                m_TextMeshPro.canvasRenderer.SetVertices(uiVertices, uiVertices.Length);
                #endregion
            }
            */
            #endregion
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log("OnPointerUp()");
        }


        void RestoreCachedVertexAttributes(int index)
        {
            if (index == -1 || index > m_TextMeshPro.textInfo.characterCount - 1) return;

            // Get the index of the material / sub text object used by this character.
            int materialIndex = m_TextMeshPro.textInfo.characterInfo[index].materialReferenceIndex;

            // Get the index of the first vertex of the selected character.
            int vertexIndex = m_TextMeshPro.textInfo.characterInfo[index].vertexIndex;

            // Restore Vertices
            // Get a reference to the cached / original vertices.
            Vector3[] src_vertices = m_cachedMeshInfoVertexData[materialIndex].vertices;

            // Get a reference to the vertices that we need to replace.
            Vector3[] dst_vertices = m_TextMeshPro.textInfo.meshInfo[materialIndex].vertices;

            // Restore / Copy vertices from source to destination
            dst_vertices[vertexIndex + 0] = src_vertices[vertexIndex + 0];
            dst_vertices[vertexIndex + 1] = src_vertices[vertexIndex + 1];
            dst_vertices[vertexIndex + 2] = src_vertices[vertexIndex + 2];
            dst_vertices[vertexIndex + 3] = src_vertices[vertexIndex + 3];

            // Restore Vertex Colors
            // Get a reference to the vertex colors we need to replace.
            Color32[] dst_colors = m_TextMeshPro.textInfo.meshInfo[materialIndex].colors32;

            // Get a reference to the cached / original vertex colors.
            Color32[] src_colors = m_cachedMeshInfoVertexData[materialIndex].colors32;

            // Copy the vertex colors from source to destination.
            dst_colors[vertexIndex + 0] = src_colors[vertexIndex + 0];
            dst_colors[vertexIndex + 1] = src_colors[vertexIndex + 1];
            dst_colors[vertexIndex + 2] = src_colors[vertexIndex + 2];
            dst_colors[vertexIndex + 3] = src_colors[vertexIndex + 3];

            // Restore UV0S
            // UVS0
            Vector2[] src_uv0s = m_cachedMeshInfoVertexData[materialIndex].uvs0;
            Vector2[] dst_uv0s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs0;
            dst_uv0s[vertexIndex + 0] = src_uv0s[vertexIndex + 0];
            dst_uv0s[vertexIndex + 1] = src_uv0s[vertexIndex + 1];
            dst_uv0s[vertexIndex + 2] = src_uv0s[vertexIndex + 2];
            dst_uv0s[vertexIndex + 3] = src_uv0s[vertexIndex + 3];

            // UVS2
            Vector2[] src_uv2s = m_cachedMeshInfoVertexData[materialIndex].uvs2;
            Vector2[] dst_uv2s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs2;
            dst_uv2s[vertexIndex + 0] = src_uv2s[vertexIndex + 0];
            dst_uv2s[vertexIndex + 1] = src_uv2s[vertexIndex + 1];
            dst_uv2s[vertexIndex + 2] = src_uv2s[vertexIndex + 2];
            dst_uv2s[vertexIndex + 3] = src_uv2s[vertexIndex + 3];


            // Restore last vertex attribute as we swapped it as well
            int lastIndex = (src_vertices.Length / 4 - 1) * 4;

            // Vertices
            dst_vertices[lastIndex + 0] = src_vertices[lastIndex + 0];
            dst_vertices[lastIndex + 1] = src_vertices[lastIndex + 1];
            dst_vertices[lastIndex + 2] = src_vertices[lastIndex + 2];
            dst_vertices[lastIndex + 3] = src_vertices[lastIndex + 3];

            // Vertex Colors
            src_colors = m_cachedMeshInfoVertexData[materialIndex].colors32;
            dst_colors = m_TextMeshPro.textInfo.meshInfo[materialIndex].colors32;
            dst_colors[lastIndex + 0] = src_colors[lastIndex + 0];
            dst_colors[lastIndex + 1] = src_colors[lastIndex + 1];
            dst_colors[lastIndex + 2] = src_colors[lastIndex + 2];
            dst_colors[lastIndex + 3] = src_colors[lastIndex + 3];

            // UVS0
            src_uv0s = m_cachedMeshInfoVertexData[materialIndex].uvs0;
            dst_uv0s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs0;
            dst_uv0s[lastIndex + 0] = src_uv0s[lastIndex + 0];
            dst_uv0s[lastIndex + 1] = src_uv0s[lastIndex + 1];
            dst_uv0s[lastIndex + 2] = src_uv0s[lastIndex + 2];
            dst_uv0s[lastIndex + 3] = src_uv0s[lastIndex + 3];

            // UVS2
            src_uv2s = m_cachedMeshInfoVertexData[materialIndex].uvs2;
            dst_uv2s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs2;
            dst_uv2s[lastIndex + 0] = src_uv2s[lastIndex + 0];
            dst_uv2s[lastIndex + 1] = src_uv2s[lastIndex + 1];
            dst_uv2s[lastIndex + 2] = src_uv2s[lastIndex + 2];
            dst_uv2s[lastIndex + 3] = src_uv2s[lastIndex + 3];

            // Need to update the appropriate 
            m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
    }
}
