using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }
    //기능성 함수를 넣어주는 함수
    public static T FindChild<T>(GameObject go,string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if(go == null)
            return null;
        //재귀함수로 자식의 자식까지 찾을건지 버전2개 만들어야함 재귀불허용 인것과 재귀 허용
        if(recursive == false)
        {   //직속 자식만 찾기
            for (int i = 0; i < go.transform.childCount; i++)
            {
                //GetChild(0) 0은 몇번째 자식을 찾는지 0은 직속자식만
                Transform transform = go.transform.GetChild(0);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        
        }
        else
        {
            //재귀적으로 자식의 자식의 자식....까지 찾기
            foreach(T component in go.GetComponentsInChildren<T>())
            { 
                if(string.IsNullOrEmpty(name) || component.name == name) 
                    return component;

            }
        }

        return null;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if(transform == null) return null;
        return transform.gameObject;
    }
}
