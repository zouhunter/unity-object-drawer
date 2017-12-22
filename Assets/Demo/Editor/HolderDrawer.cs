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
using Drawer_object;

[CustomEditor(typeof(HolderBehaiver))]
public class HolderDrawer : Editor
{
    HolderBehaiver holder;
    Serialized_object<ContentClass> serialize_obj;
    Serialized_property intprop;
    Serialized_property stringprop;
    private void OnEnable()
    {
        holder = target as HolderBehaiver;
        serialize_obj = new Serialized_object<ContentClass>(holder.content);
        intprop = serialize_obj.FindProperty("intTest");
        stringprop = serialize_obj.FindProperty("stringTest");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout_object.Serialized_objectField(serialize_obj);
        EditorGUILayout_object.PropertyField(intprop);
        EditorGUILayout_object.PropertyField(stringprop);
        serialize_obj.SetDirty();
    }
}
