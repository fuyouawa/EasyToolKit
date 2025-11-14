using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Core
{
    [Serializable]
    public class TransformRecorder
    {
        [SerializeField] private Vector3 _localPosition;
        [SerializeField] private Quaternion _localRotation;
        [SerializeField] private Vector3 _localScale;
        [SerializeField] private Transform _parent;

        public void Record(Transform target)
        {
            _localPosition = target.localPosition;
            _localRotation = target.localRotation;
            _localScale = target.localScale;
            _parent = target.parent;
        }

        public void Resume(Transform target)
        {
            target.SetParent(_parent);
            target.localPosition = _localPosition;
            target.localRotation = _localRotation;
            target.localScale = _localScale;
        }
    }

    public static class TransformExtensions
    {
        public static bool IsParentRecursive(this Transform transform, Transform parent)
        {
            if (transform == parent)
                return false;

            var p = transform.parent;
            while (p != parent)
            {
                if (p == null)
                    return false;
                p = p.parent;
            }

            return true;
        }

        public static T[] FindObjectsByTypeInParents<T>(this Transform transform, bool includeInactive = false, bool includeSelf = false)
        {
            var total = new List<T>();

            var p = includeSelf ? transform : transform.parent;
            while (p != null)
            {
                var c = p.GetComponent<T>();
                if (c != null)
                {
                    total.Add(c);
                }

                p = p.parent;
            }

            return total.ToArray();
        }

        public static TransformRecorder GetRecorder(this Transform transform)
        {
            var recorder = new TransformRecorder();
            recorder.Record(transform);
            return recorder;
        }

        // public static IEnumerable<Transform> FindParents(this Transform transform, Func<Transform, bool> condition)
        // {
        //     var ret = new List<Transform>();
        //
        //     var p = transform.parent;
        //     while (p != null)
        //     {
        //         if (condition(p))
        //         {
        //             ret.Add(p);
        //         }
        //         p = p.parent;
        //     }
        //
        //     return ret;
        // }
        //
        // public static void ForEachParentRecursive(this Transform transform, Func<Transform, bool> predicate)
        // {
        //     var p = transform.parent;
        //     while (p != null)
        //     {
        //         if (!predicate(p))
        //             return;
        //         p = p.parent;
        //     }
        // }

        public static float ScaleSquare(this Transform transform)
        {
            return transform.localScale.magnitude;
        }

        public static void SetScaleSquare(this Transform transform, float size)
        {
            transform.localScale = transform.localScale.normalized * size;
        }

        public static void SetPositionXY(this Transform transform, Vector3 position)
        {
            SetPositionXY(transform, position.ToVector2());
        }

        public static void SetPositionXY(this Transform transform, Vector2 position)
        {
            transform.position = position.ToVector3(transform.position.z);
        }


        public static string GetRelativePath(this Transform transform, Transform parent, bool includeParent = true)
        {
            if (transform == null)
                return string.Empty;
            var hierarchy = new Stack<string>();

            var p = transform;
            while (p != null && p != parent)
            {
                hierarchy.Push(p.gameObject.name);
                p = p.parent;
            }

            if (includeParent && parent != null)
            {
                hierarchy.Push(parent.gameObject.name);
            }

            var path = string.Join("/", hierarchy);

            if (parent == null)
                path = '/' + path;

            return path;
        }

        public static string GetAbsolutePath(this Transform transform, bool includeSceneName = true)
        {
            if (transform == null)
                return string.Empty;
            var path = GetRelativePath(transform, null);

            if (includeSceneName)
                path = '/' + transform.gameObject.scene.name + path;

            return path;
        }

        public static void DestroyChildren(this Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                UnityEngine.Object.Destroy(transform.GetChild(transform.childCount - i - 1).gameObject);
            }
        }
    }
}
