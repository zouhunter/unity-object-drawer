using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Reflection;

namespace Drawer_object
{
    public class Serialized_object
    {
        private Serialized_property obj_Prop;

        public object targetObject { get; private set; }

        internal bool hasModifiedProperties { get; private set; }

        public Serialized_object(FieldInfo info,object holder)
        {
            this.targetObject = holder;
            obj_Prop = new Serialized_property(info,holder);
            obj_Prop.serializedObject = this;
        }

        public void Update() {

        }

        public void SetIsDifferentCacheDirty() { }

        internal Serialized_property GetIterator()
        {
            return obj_Prop;
        }

        public void UpdateIfDirtyOrScript() { }

        public void Dispose() { }

        ~Serialized_object()
        {
            this.Dispose();
        }

        public Serialized_property FindProperty(string propertyPath)
        {
            return obj_Prop.FindPropertyInternal(propertyPath);
        }

        private  Property_modification ExtractPropertyModification(string propertyPath) { return null; }

        
        public  bool ApplyModifiedProperties() { return true; }
    }
}