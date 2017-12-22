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
    Serialized_object serialize_obj;
    Serialized_property prop;
    private void OnEnable()
    {
        holder = target as HolderBehaiver;
        InitObject();
        serialize_obj = new Serialized_object(holder.GetType().GetField("content"),holder);
        prop = serialize_obj.FindProperty("intTest");
    }
    private void InitObject()
    {
        if (!string.IsNullOrEmpty(holder.instenceData)){
            var item = (ContentClass)JsonUtility.FromJson(holder.instenceData, holder.content.GetType());
            if(item != null && item.GetType() == holder.content.GetType())
            {
                holder.content = item;
            }
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(EditorGUILayout_object.Serialized_objectField(serialize_obj)){
            holder.instenceData = JsonUtility.ToJson(holder.content);
        }
        
    }
}
