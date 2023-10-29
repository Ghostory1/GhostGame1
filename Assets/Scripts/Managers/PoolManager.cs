using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PoolManager 
{
    #region Pool
    //UnityChan_Root 같은것의 자식들
    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        Stack<Poolable> _poolstack = new Stack<Poolable>();

        public void Init(GameObject original, int count = 5)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"{original.name}_Root";

            for(int i = 0; i < count; i++)
            {
                Push(Create());
            }
        }

        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;
            return go.GetOrAddComponent<Poolable>();
        }

        public void Push(Poolable poolable)
        {
            if (poolable == null)
                return;

            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            poolable.IsUsing = false;

            _poolstack.Push(poolable);
        }

        public Poolable Pop(Transform parent)
        {
            Poolable poolable;
            if (_poolstack.Count > 0)
                poolable = _poolstack.Pop();
            else
                poolable = Create();

            poolable.gameObject.SetActive(true);
            //DontDestroyOnLoad 해제 용도
            if (parent == null)
                poolable.transform.parent = Managers.Scene.CurrentScene.transform;
            poolable.transform.parent = parent;
            poolable.IsUsing = true;
            return poolable;
        }
    }
    #endregion
    //PoolManager은 여러개의 Pool들을 가지고 있는데 Dictionary에서 string이라는 키로 관리
    //@Pool_Root 가 PoolManager이고 UnityChan_Root 나 Tank_Root 같은 Pool을 Dictionary로 관리하고 string 키 이름을 통해 찾아서 Pool클래스 생성
    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
    Transform _root;
    
    public void Init()
    {
        if(_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }

    public void CreatePool(GameObject original,int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.parent = _root;

        _pool.Add(original.name, pool);
    }

    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if(_pool.ContainsKey(name)==false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }
        _pool[name].Push(poolable);
    }
    public Poolable Pop(GameObject original, Transform parent = null) 
    {
        if (_pool.ContainsKey(original.name) == false)
            CreatePool(original);
        return _pool[original.name].Pop(parent);
    }
    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;
        return _pool[name].Original;
    }

    public void Clear()
    {
        //@Pool_Root 산하의 차일드를 다 삭제
        foreach(Transform child in _root)
            GameObject.Destroy(child.gameObject);

        _pool.Clear();
    }
}
