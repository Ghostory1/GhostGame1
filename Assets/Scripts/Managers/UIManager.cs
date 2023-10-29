using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    //UIManager 역할 : 팝업창을 껏다 켰다 했을때 Sort Order에 따라 먼저 끌수있는 것을 구분하기 위해

    int _order = 10;
    //선입선출의 팝업 순서이니 Stack 으로 관리, Stack에 내용은 자기가 사용하고있는 컴포넌트 자체를 들고있는게 낫다.
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
        //외부에서 팝업이 켜졌을 때 역으로 UIManager한테 SetCanavs를 요청해서 자기 Canvas에 기존 UI와 우선순위를 정함
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //overrideSoring은 캔버스안에 캔버스가 중첩되어 있으면 부모가 어떤값을 가지던 자기 소팅값을 가질거라는 옵션
        canvas.overrideSorting = true;
        if(sort)
        {
            //sort true 요청한상황
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            //sort false는 결국 팝업Stack<UI_Popup>이랑 연관이 없는 일반 UI라는 말이 되므로,
            canvas.sortingOrder = 0;
        }
    }

    //sceneUI
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        //아무런 이름이 안들어왔을때 템플릿 이름을 추출해서 넣어준다.
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");

        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        //루트가 없으면 똑같은 게임 오브젝트를 만들고 게임 오브젝트 산하에 우리 오브젝트를 연결
        go.transform.SetParent(Root.transform);

        return sceneUI;
    }
    //SubItem
    public T MakeSubItem<T>(Transform parent = null, string name = null) where T: UI_Base
    {
        //아무런 이름이 안들어왔을때 템플릿 이름을 추출해서 넣어준다.
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");

        if (parent != null)
             go.transform.SetParent(parent);
        
        return Util.GetOrAddComponent<T>(go);
    }


    //팝업UI을 띄워보자
    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        //아무런 이름이 안들어왔을때 템플릿 이름을 추출해서 넣어준다.
        if(string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");

         T popup= Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        //루트가 없으면 똑같은 게임 오브젝트를 만들고 게임 오브젝트 산하에 우리 오브젝트를 연결
        go.transform.SetParent(Root.transform);

        return popup;
    }

    //원하는애를 삭제가 되는지 예방
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


    //팝업 닫기
    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;
        UI_Popup popup = _popupStack.Pop();
        //예를 들어 UI_Button 스크립트를 가지고있는 GameObejct는 UI_Button이니까 삭제 시키고 더이상 접근하지못하게 null
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
