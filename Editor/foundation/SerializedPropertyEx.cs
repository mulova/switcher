//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public static class SerializedPropertyEx
    {
        public static object GetValue(this SerializedProperty p)
        {
            switch (p.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return p.intValue;
                case SerializedPropertyType.Boolean:
                    return p.boolValue;
                case SerializedPropertyType.Float:
                    return p.floatValue;
                case SerializedPropertyType.String:
                    return p.stringValue;
                case SerializedPropertyType.Color:
                    return p.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return p.objectReferenceValue;
                case SerializedPropertyType.LayerMask:
                    return p.intValue;
                case SerializedPropertyType.Enum:
                    return p.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return p.vector2Value;
                case SerializedPropertyType.Vector3:
                    return p.vector3Value;
                case SerializedPropertyType.Vector4:
                    return p.vector4Value;
                case SerializedPropertyType.Rect:
                    return p.rectValue;
                case SerializedPropertyType.ArraySize:
                    return p.arraySize;
                case SerializedPropertyType.Character:
                    return p.intValue;
                case SerializedPropertyType.AnimationCurve:
                    return p.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return p.boundsValue;
                case SerializedPropertyType.Gradient:
                    return p.colorValue;
                case SerializedPropertyType.Quaternion:
                    return p.quaternionValue;
                case SerializedPropertyType.ExposedReference:
                    return p.exposedReferenceValue;
                case SerializedPropertyType.FixedBufferSize:
                    return p.fixedBufferSize;
                case SerializedPropertyType.Vector2Int:
                    return p.vector2IntValue;
                case SerializedPropertyType.Vector3Int:
                    return p.vector3IntValue;
                case SerializedPropertyType.RectInt:
                    return p.rectIntValue;
                case SerializedPropertyType.BoundsInt:
                    return p.boundsIntValue;
                default:
                    return default;
                    //throw new Exception("Unreachable");
            }
        }

        public static void SetValue(this SerializedProperty p, object val)
        {
            switch (p.propertyType)
            {
                case SerializedPropertyType.Integer:
                    p.intValue = (int)val;
                    break;
                case SerializedPropertyType.Boolean:
                    p.boolValue = (bool)val;
                    break;
                case SerializedPropertyType.Float:
                    p.floatValue = (float)val;
                    break;
                case SerializedPropertyType.String:
                    p.stringValue = (string)val;
                    break;
                case SerializedPropertyType.Color:
                    p.colorValue = (Color)val;
                    break;
                case SerializedPropertyType.ObjectReference:
                    p.objectReferenceValue = (UnityEngine.Object)val;
                    break;
                case SerializedPropertyType.LayerMask:
                    p.intValue = (int)val;
                    break;
                case SerializedPropertyType.Enum:
                    p.enumValueIndex = (int)val;
                    break;
                case SerializedPropertyType.Vector2:
                    p.vector2Value = (Vector2)val;
                    break;
                case SerializedPropertyType.Vector3:
                    p.vector3Value = (Vector3)val;
                    break;
                case SerializedPropertyType.Vector4:
                    p.vector4Value = (Vector4)val;
                    break;
                case SerializedPropertyType.Rect:
                    p.rectValue = (Rect)val;
                    break;
                case SerializedPropertyType.ArraySize:
                    p.arraySize = (int)val;
                    break;
                case SerializedPropertyType.Character:
                    p.intValue = (int)val;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    p.animationCurveValue = (AnimationCurve)val;
                    break;
                case SerializedPropertyType.Bounds:
                    p.boundsValue = (Bounds)val;
                    break;
                case SerializedPropertyType.Gradient:
                    p.colorValue = (Color)val;
                    break;
                case SerializedPropertyType.Quaternion:
                    p.quaternionValue = (Quaternion)val;
                    break;
                case SerializedPropertyType.ExposedReference:
                    p.exposedReferenceValue = (Object)val;
                    break;
                //case SerializedPropertyType.FixedBufferSize:
                //    p.fixedBufferSize = val;
                //    break;
                case SerializedPropertyType.Vector2Int:
                    p.vector2IntValue = (Vector2Int)val;
                    break;
                case SerializedPropertyType.Vector3Int:
                    p.vector3IntValue = (Vector3Int)val;
                    break;
                case SerializedPropertyType.RectInt:
                    p.rectIntValue = (RectInt)val;
                    break;
                case SerializedPropertyType.BoundsInt:
                    p.boundsIntValue = (BoundsInt)val;
                    break;
                default:
                    break;
                    //throw new Exception("Unreachable");
            }
        }
    }
}

