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
using Drawer_object;
[System.Serializable]
public class Temp : SerializedInstance<ContentClass>
{
    public Temp(ContentClass obj) :base(obj) { }
    public Temp(SerializedInstance<ContentClass> instence) :base(instence) { }
}

public class HolderBehaiver : MonoBehaviour {

    public Temp content = new Temp(new ContentA());

    private void Start()
    {
        if(content.Object is ContentA)
        {
            var a = content.Object as ContentA;
            Debug.Log(a.stringTest);
        }
        else if(content.Object is ContentB)
        {
            var b = content.Object as ContentB;
            Debug.Log(b.boolTest);
        }
    }
}
