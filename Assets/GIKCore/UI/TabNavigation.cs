using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GIKCore.UI
{
    public class TabNavigation : MonoBehaviour
    {
        // Fields
        [SerializeField] private List<Selectable> m_ListSelectable = new List<Selectable>();
        [SerializeField] private bool m_IsWrapAround = true;

        // Values
        private Selectable lastSelectable = null;

        // Methods
        private void HandleHotkeySelect(bool isNavigateBackward)
        {
            GameObject selectedGo = (lastSelectable != null) ? lastSelectable.gameObject : null; //EventSystem.current.currentSelectedGameObject;
            // Ensure a selection exists and is not an inactive object.
            if (selectedGo != null && selectedGo.activeInHierarchy)
            {
                Selectable currentSelection = selectedGo.GetComponent<Selectable>();
                if (currentSelection != null)
                {
                    Selectable nextSelection = FindNextSelectable(m_ListSelectable.IndexOf(currentSelection), isNavigateBackward);
                    SetSelectable(nextSelection);                    
                }
                else SelectFirstSelectable();
            }
            else SelectFirstSelectable();
        }
        private void SelectFirstSelectable()
        {
            if (m_ListSelectable != null && m_ListSelectable.Count > 0)
            {
                SetSelectable(m_ListSelectable[0]);                
            }
        }
        /// <summary>
        /// Looks at ordered selectable list to find the selectable we are trying to navigate to and returns it.
        /// </summary>
        private Selectable FindNextSelectable(int currentSelectableIndex, bool isNavigateBackward)
        {
            Selectable nextSelection = null;
            int totalSelectables = m_ListSelectable.Count;
            if (totalSelectables > 1)
            {
                if (isNavigateBackward)
                {
                    if (currentSelectableIndex == 0)
                        nextSelection = (m_IsWrapAround) ? m_ListSelectable[totalSelectables - 1] : null;
                    else nextSelection = m_ListSelectable[currentSelectableIndex - 1];
                }
                else
                {// Navigate forward.
                    if (currentSelectableIndex == (totalSelectables - 1))
                        nextSelection = (m_IsWrapAround) ? m_ListSelectable[0] : null;
                    else
                        nextSelection = m_ListSelectable[currentSelectableIndex + 1];
                }
            }

            return nextSelection;
        }

        private void SetSelectable(Selectable target)
        {
            if (target != null)
            {
                lastSelectable = target;
                target.Select();
            }                        
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        void Update()
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                // Navigate backward when holding shift, else navigate forward.
                HandleHotkeySelect(Keyboard.current.shiftKey.isPressed);
            }
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                EventSystem.current.SetSelectedGameObject(null, null);
            }
        }

        void OnDisable()
        {
            lastSelectable = null;    
        }
    }
}