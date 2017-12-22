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

public abstract class ContentClass {

}

[System.Serializable]
public class ContentA: ContentClass
{
    public int intTest;
    public string stirngTest;
    public GameObject objectTest;
    //public List<ContentA> classTest;
}


[System.Serializable]
public class ContentB : ContentClass
{
    public bool boolTest;
    public float floatTest;
}
