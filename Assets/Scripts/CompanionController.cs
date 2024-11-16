using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : TeleportableObjects
{
    public static Action OnCubeDestroyed;

    private void OnDestroy()
    {
        OnCubeDestroyed?.Invoke();
    }
}
