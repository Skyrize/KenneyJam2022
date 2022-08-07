using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension {
    public static void DestroyChilds(this Transform _transform) {
        while (_transform.childCount != 0) {
            if (Application.isPlaying) {
                GameObject.Destroy(_transform.GetChild(0).gameObject);
            } else {
                GameObject.DestroyImmediate(_transform.GetChild(0).gameObject);
            }
        }
    }
    
    public static Transform[] GetAllChildren(this Transform _transform)
    {
        Transform[] children = new Transform[_transform.childCount];

        for (int i = 0; i != _transform.childCount; i++) {
            children[i] = _transform.GetChild(i);
        }
        return children;
    }

    public static Transform GetRandomChild(this Transform _transform)
    {
        return _transform.GetChild(Random.Range(0, _transform.childCount));
    }

    public static T[] GetComponentsInDirectChildren<T>(this Transform _transform, bool _includeDisabled = false)
    {
        List<T> result = new List<T>();

        for (int i = 0; i != _transform.childCount; i++) {
            var child = _transform.GetChild(i);
            if (!_includeDisabled && !child.gameObject.activeInHierarchy)
                continue;
            T comp = child.GetComponent<T>();
            if (comp != null)
                result.Add(comp);
        }
        return result.ToArray();
    }
}
