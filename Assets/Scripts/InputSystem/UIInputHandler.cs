using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInputHandler : MonoBehaviour, IInputHandler
{
    void IInputHandler.ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action callback)
    {
        callback?.Invoke();
    }
}
