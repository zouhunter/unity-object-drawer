using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;

namespace Drawer_object
{
    public static class EditorGUILayout_object
    {
        public static bool Serialized_objectField(Serialized_object s_obj)
        {
            s_obj.Update();
            Serialized_property iterator = s_obj.GetIterator();
            bool enterChildren = true;
            var changed = false;
            while ((iterator = iterator.NextVisible(enterChildren)) != null)
            {
                EditorGUI.BeginDisabledGroup("m_Script" == iterator.propertyPath);
                changed |= PropertyField(iterator, new GUILayoutOption[0]);
                EditorGUI.EndDisabledGroup();
                enterChildren = false;
            }
            if (changed)
            {
                s_obj.ApplyModifiedProperties();
            }
            return changed;
        }

        public static bool Serialized_objectField(Serialized_object s_obj, GUIContent label)
        {
            return Serialized_objectField(s_obj);
        }

        public static bool PropertyField(Serialized_property property, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(property.name, GUILayout.Width(100));
            EditorGUI.BeginChangeCheck();
            property.value = DrawField(property.value);
            EditorGUILayout.EndHorizontal();

            return EditorGUI.EndChangeCheck();
        }
        public static bool PropertyField(Serialized_property property, GUIContent label, params GUILayoutOption[] options)
        {
            return true;
        }

        public static object DrawClassObject(FieldInfo field, object classItem, Dictionary<FieldInfo, bool> toggleDic, Dictionary<FieldInfo, List<FieldInfo>> fieldDic)
        {
            if (classItem == null || toggleDic == null || fieldDic == null || !fieldDic.ContainsKey(field)) return null;

            EditorGUI.indentLevel++;
            if (GUILayout.Button(classItem.GetType().Name, EditorStyles.boldLabel))
            {
                toggleDic[field] = !toggleDic[field];
            }

            if (toggleDic[field])
            {
                foreach (var item in fieldDic[field])
                {
                    if (item.FieldType.IsValueType || item.FieldType == typeof(string))
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(item.Name, GUILayout.Width(100));
                        item.SetValue(classItem, DrawField(item.GetValue(classItem)));
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (item.FieldType.IsGenericType || item.FieldType.IsArray) continue;
                    else if (item.FieldType.IsClass && (item.FieldType != typeof(string)))
                    {
                        DrawClassObject(item, item.GetValue(classItem), toggleDic, fieldDic);
                    }
                }
            }

            return classItem;
        }

        public static object DrawField(object data)
        {
            if (data is int)
            {
                data = EditorGUILayout.IntField(Convert.ToInt32(data));
            }
            else if (data is bool)
            {
                data = EditorGUILayout.Toggle(Convert.ToBoolean(data));
            }
            else if (data is float || data is double)
            {
                data = EditorGUILayout.FloatField(float.Parse(data.ToString()));
            }
            else if (data is string)
            {
                data = EditorGUILayout.TextField(data.ToString());
            }
            else if (data is Color)
            {
                data = EditorGUILayout.ColorField((Color)data);
            }
            else if (data is Enum)
            {
                data = EditorGUILayout.EnumPopup((Enum)data);
            }
            else if (data is Vector2)
            {
                data = EditorGUILayout.Vector2Field("", (Vector2)data);
            }
            else if (data is Vector3)
            {
                data = EditorGUILayout.Vector3Field("", (Vector3)data);
            }
            else if (data is Vector4)
            {
                data = EditorGUILayout.Vector4Field("", (Vector4)data);
            }
            else if (data is Rect)
            {
                data = EditorGUILayout.RectField("", (Rect)data);
            }
            return data;
        }


    }
}