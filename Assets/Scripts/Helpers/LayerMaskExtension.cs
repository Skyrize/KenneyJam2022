using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtension
{
    public static bool ContainsLayer(this LayerMask _mask, int _layer)
    {
        return (_mask.value & (1 << _layer)) > 0;
    }
}
