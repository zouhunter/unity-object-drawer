using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Internal;
using System.Reflection;
using Debug = UnityEngine.Debug;

namespace Drawer_object
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Serialized_property
    {
        internal Serialized_object m_SerializedObject;
        public Serialized_object serializedObject
        {
            get
            {
                return this.m_SerializedObject;
            }
            set
            {
                m_SerializedObject = value;
            }
        }
        public Serialized_property parentProp { get; private set; }

        public FieldInfo fieldInfo;

        private object content;

        private List<Serialized_property> subProps;

        public object value { get { return fieldInfo.GetValue(content); }set { fieldInfo.SetValue(content,value); } }

        public bool isExpanded { get; set; }

        public string displayName { get { return name; } }

        public string name { get { return fieldInfo.Name; } }

        public string type { get { return fieldInfo.FieldType.ToString(); } }

        public string tooltip { get; set; }

        public int depth { get; set; }

        public string propertyPath { get; private set; }

        internal int hashCodeForPropertyPathWithoutArrayIndex;

        public bool editable { get { return true; } }

        public bool isAnimated;


        public bool hasChildren { get { return subProps != null && subProps.Count > 0; } }

        public bool hasVisibleChildren { get { return hasChildren && subProps.Find(x => x.editable) != null; } }

        public Serialized_propertyType propertyType { get; private set; }

        public int intValue { get { return (propertyType == Serialized_propertyType.Integer) ? (int)value : 0; } }

        public long longValue { get { return (propertyType == Serialized_propertyType.Integer) ? (long)value : 0; } }

        public bool boolValue { get { return (propertyType == Serialized_propertyType.Boolean) ? (bool)value : false; } }

        public float floatValue { get { return (propertyType == Serialized_propertyType.Float) ? (float)value : 0; } }

        public double doubleValue { get { return (propertyType == Serialized_propertyType.Float) ? (double)value : 0; } }

        public string stringValue;

        public Color colorValue { get { return new Color(); } }

        public AnimationCurve animationCurveValue;

        internal Gradient gradientValue;

        public object objectReferenceValue;

        public int objectReferenceInstanceIDValue;

        internal string objectReferenceStringValue;

        internal string objectReferenceTypeString;

        internal string layerMaskStringValue;

        public int enumValueIndex;

        public string[] enumNames;
        public string[] enumDisplayNames;
        public Vector2 vector2Value;

        public Vector3 vector3Value;

        public Vector4 vector4Value { get { return new Vector4(); } }

        public Quaternion quaternionValue { get { return new Quaternion(); } }

        public Rect rectValue { get { return new Rect(); } }

        public Bounds boundsValue { get { return new Bounds(); } }

        public bool isArray { get { return fieldInfo.FieldType.IsArrayOrList(); } }

        public int arraySize;

        internal Serialized_property(FieldInfo info, object holder)
        {
            this.fieldInfo = info;
            this.content = holder;
            subProps = GetSubPropopertys(fieldInfo, holder);
        }

        public void SetParentProperty(Serialized_property parent)
        {
            this.parentProp = parent;
            if (!string.IsNullOrEmpty(parent.propertyPath))
            {
                propertyPath = parent.propertyPath + "/" + fieldInfo.Name;
            }
            else
            {
                propertyPath = fieldInfo.Name;
            }
        }

        public List<Serialized_property> GetSubPropopertys(FieldInfo field, object holder)
        {
            List<Serialized_property> list = new List<Drawer_object.Serialized_property>();

            var type = field.FieldType;

            if (field.GetValue(holder) == null && type.IsClass && type  != typeof(string))
            {
                field.SetValue( holder , Activator.CreateInstance(type));
            }

            if(field.GetValue(holder) == null && type == typeof(string))
            {
                field.SetValue(holder, "");
            }

            if(type.IsClass && type != typeof(string))
            {
                var value = field.GetValue(holder);
                if(value != null)
                {
                    FieldInfo[] fields = value.GetType().GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (var item in fields)
                    {
                        if (!IsFieldNeed(item)) continue;
                       
                        var prop = new Serialized_property(item, value);
                        prop.SetParentProperty(this);
                        list.Add(prop);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 判断寡字段能否序列化
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static bool IsFieldNeed(FieldInfo fieldInfo)
        {
            var type = fieldInfo.FieldType;

            //排除字典
            if (type.IsGenericType && type.Name.Contains("Dictionary`"))
            {
                return false;
            }

            //排除非公有变量
            if (fieldInfo.Attributes != FieldAttributes.Public)
            {
                var attrs = fieldInfo.GetCustomAttributes(false);
                if (attrs.Length == 0 || (attrs.Length > 0 && Array.Find(attrs, x => x is SerializeField) == null))
                {
                    return false;
                }
            }

            //排出接口
            if (type.IsInterface)
            {
                return false;
            }

            //修正type
            if (type.IsArray || type.IsGenericType)
            {
                if (type.IsGenericType)
                {
                    type = type.GetGenericArguments()[0];
                }
                else
                {
                    type = type.GetElementType();
                }
            }

            //排出修正后的接口
            if (type.IsInterface)
            {
                return false;
            }

            //排除不能序列化的类
            if (type.IsClass)
            {
                if (!type.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    var atts = type.GetCustomAttributes(false);
                    var seri = Array.Find(atts, x => x is System.SerializableAttribute);
                    if (seri == null)
                    {
                        return false;
                    }
                }
            }

            //排除内置变量
            if (fieldInfo.Name.Contains("k__BackingField"))
            {
                return false;
            }

            return true;
        }

        ~Serialized_property()
        {
            this.Dispose();
        }

        public void Dispose() { }

        public static bool EqualContents(Serialized_property x, Serialized_property y) { return false; }

        internal void SetBitAtIndexForAllTargetsImmediate(int index, bool value) { }

        public Serialized_property Next(bool enterChildren)
        {
            if (enterChildren)
            {
                if (hasChildren)
                {
                    return subProps[0];
                }
                return null;
            }
            else
            {
                var currid = parentProp.subProps.IndexOf(this);
                if (currid < parentProp.subProps.Count - 1)
                {
                    return subProps[currid + 1];
                }
                return null;
            }
        }

        public Serialized_property NextVisible(bool enterChildren)
        {
            if (enterChildren)
            {
                if (hasVisibleChildren)
                {
                    for (int i = 0; i < subProps.Count; i++)
                    {
                        if (subProps[i].editable)
                        {
                            return subProps[i];
                        }
                    }
                }
                return null;
            }
            else
            {
                var currid = parentProp.subProps.IndexOf(this);
                if (currid < parentProp.subProps.Count)
                {
                    for (int i = currid + 1; i < parentProp.subProps.Count; i++)
                    {
                        if (parentProp.subProps[i].editable)
                        {
                            return parentProp.subProps[i];
                        }
                    }
                }
                return null ;
            }
        }

        public void Reset()
        {
        }

        public int CountRemaining() { return parentProp.CountInProperty() - parentProp.subProps.IndexOf(this) - 1; }

        public int CountInProperty() { return subProps == null ? 0 : subProps.Count; }

        public Serialized_property Copy()
        {
            Serialized_property serializedProperty = new Serialized_property(fieldInfo, content);
            serializedProperty.m_SerializedObject = this.m_SerializedObject;
            return serializedProperty;
        }

        public bool DuplicateCommand() { return false; }

        public bool DeleteCommand() { return false; }

        public Serialized_property FindPropertyRelative(string relativePropertyPath)
        {
            Serialized_property serializedProperty = this.Copy();
            return serializedProperty.FindPropertyRelativeInternal(relativePropertyPath);
        }

        internal Serialized_property FindPropertyInternal(string propertyPath)
        {
            if (subProps != null)
            {
                var item = subProps.Find(x => x.propertyPath == propertyPath);
                if(item != null)
                {
                    return item;
                }
            }
            return null;
        }


        internal Serialized_property FindPropertyRelativeInternal(string propertyPath)
        {
            var propertyPath_full = this.propertyPath + "/" + propertyPath;
            return FindPropertyInternal(propertyPath_full);
        }

        internal int[] GetLayerMaskSelectedIndex() { return null; }

        internal string[] GetLayerMaskNames() { return null; }

        internal void ToggleLayerMaskAtIndex(int index) { }

        public Serialized_property GetArrayElementAtIndex(int index)
        {
            Serialized_property serializedProperty = this.Copy();
            if (serializedProperty.GetArrayElementAtIndexInternal(index))
            {
                return serializedProperty;
            }
            return null;
        }

        private bool GetArrayElementAtIndexInternal(int index) { return false; }


        public void InsertArrayElementAtIndex(int index) { }


        public void DeleteArrayElementAtIndex(int index) { }


        public void ClearArray() { }


        public bool MoveArrayElement(int srcIndex, int dstIndex) { return false; }

    }
}
