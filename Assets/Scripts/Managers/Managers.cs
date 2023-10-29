using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance; // static ���� ���ϼ� ����
    static Managers Instance { get { Init(); return s_instance; } } //������ �Ŵ����� ������ �´�.
    
    DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resourceManager = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();

    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource {  get { return Instance._resourceManager; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManager Sound {  get { return Instance._sound; } }  
    public static UIManager UI { get { return Instance._ui; } }

    
    // Start is called before the first frame update
    void Start()
    {
        //�ʱ�ȭ
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        _input.OnUpdate();
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if(go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            //������ �ʱ�ȭ
            s_instance._data.Init();
            //Ǯ�� �ʱ�ȭ
            s_instance._pool.Init();
            //���� �ʱ�ȭ
            s_instance._sound.Init();
            
        }
    }

    public static void Clear()
    {
        Sound.Clear();
        Input.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
    }
}