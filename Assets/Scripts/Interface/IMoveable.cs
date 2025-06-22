using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    void OnDrag();
    void OnDrop();
    void OnHover();
    void OnExit();
}
