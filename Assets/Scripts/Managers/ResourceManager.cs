using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        //Prefab일때
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;

        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if(original ==null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }
        //2.혹시 폴링된 애가 있을까 ? 있으면 찾아서 반환
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        //Poolable이 아닌 대상은 그냥 로딩
        GameObject go = Object.Instantiate(original, parent);
        int index = go.name.IndexOf("(Clone)");
        go.name = original.name;

        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go ==null)
        {
            return;
        }

        //만약에 풀링이 필요한 아이라면 -> 풀링 매니저한테 위탁
        Poolable poolable = go.GetComponent<Poolable>();
        if(poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }
        //풀링대상이 아니면 그냥 삭제 똑같이
        Object.Destroy(go);
    }
}
