using UnityEngine;

public class Sc_Set_Sorting_Order : MonoBehaviour
{
    [SerializeField] private int new_sorting_order = 0;
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.sortingOrder = new_sorting_order;
            int sortingOrder = renderer.sortingOrder;
            Debug.Log("Sorting Order: " + sortingOrder);
        }
        else
        {
            Debug.LogWarning("Renderer component not found.");
        }
    }


}
