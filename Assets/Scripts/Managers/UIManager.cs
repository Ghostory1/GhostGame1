using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    //UIManager ���� : �˾�â�� ���� �״� ������ Sort Order�� ���� ���� �����ִ� ���� �����ϱ� ����

    int _order = 10;
    //���Լ����� �˾� �����̴� Stack ���� ����, Stack�� ������ �ڱⰡ ����ϰ��ִ� ������Ʈ ��ü�� ����ִ°� ����.
    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    UI_Scene _sceneUI = null;

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }


    public void SetCanvas(GameObject go,bool sort = true)
    {
        //�ܺο��� �˾��� ������ �� ������ UIManager���� SetCanavs�� ��û�ؼ� �ڱ� Canvas�� ���� UI�� �켱������ ����
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //overrideSoring�� ĵ�����ȿ� ĵ������ ��ø�Ǿ� ������ �θ� ����� ������ �ڱ� ���ð��� �����Ŷ�� �ɼ�
        canvas.overrideSorting = true;
        if(sort)
        {
            //sort true ��û�ѻ�Ȳ
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            //sort false�� �ᱹ �˾�Stack<UI_Popup>�̶� ������ ���� �Ϲ� UI��� ���� �ǹǷ�,
            canvas.sortingOrder = 0;
        }
    }

    //sceneUI
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        //�ƹ��� �̸��� �ȵ������� ���ø� �̸��� �����ؼ� �־��ش�.
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");

        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        //��Ʈ�� ������ �Ȱ��� ���� ������Ʈ�� ����� ���� ������Ʈ ���Ͽ� �츮 ������Ʈ�� ����
        go.transform.SetParent(Root.transform);

        return sceneUI;
    }
    //SubItem
    public T MakeSubItem<T>(Transform parent = null, string name = null) where T: UI_Base
    {
        //�ƹ��� �̸��� �ȵ������� ���ø� �̸��� �����ؼ� �־��ش�.
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");

        if (parent != null)
             go.transform.SetParent(parent);
        
        return Util.GetOrAddComponent<T>(go);
    }


    //�˾�UI�� �������
    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        //�ƹ��� �̸��� �ȵ������� ���ø� �̸��� �����ؼ� �־��ش�.
        if(string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");

         T popup= Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        //��Ʈ�� ������ �Ȱ��� ���� ������Ʈ�� ����� ���� ������Ʈ ���Ͽ� �츮 ������Ʈ�� ����
        go.transform.SetParent(Root.transform);

        return popup;
    }

    //���ϴ¾ָ� ������ �Ǵ��� ����
    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;
        if(_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed");
            return;
        }
        ClosePopupUI();
    }


    //�˾� �ݱ�
    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;
        UI_Popup popup = _popupStack.Pop();
        //���� ��� UI_Button ��ũ��Ʈ�� �������ִ� GameObejct�� UI_Button�̴ϱ� ���� ��Ű�� ���̻� �����������ϰ� null
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
    }

    public void CloseAllPopup()
    {
        while(_popupStack.Count>0)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopup();
        _sceneUI = null;
    }
}
