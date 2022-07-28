using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Helpers
{
    public static void VisitChildren(Transform _transform, Action<Transform> _lambda)
    {
        foreach (Transform child in _transform)
        {
            _lambda(child);
        }
    }

    public static void VisitChildrenRecursively(Transform _transform, Action<Transform> _lambda)
    {
        foreach (Transform child in _transform)
        {
            _lambda(child);
            VisitChildrenRecursively(child, _lambda);
        }
    }
}