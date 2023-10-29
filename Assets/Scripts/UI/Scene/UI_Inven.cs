using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inven : UI_Scene
{
    enum GameObjects
    {
        GridPanel
    }
    void Start()
    {
        Init();
    }


    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        GameObject gridPanel = Get<GameObject>((int)GameObjects.GridPanel);
        
        foreach(Transform child in gridPanel.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }
        //실제 인벤토리 정보를 참고해서 넣어주는 작업
        for(int i = 0; i < 8; i++)
        {
            
            //프리팹 만들기
            GameObject item = Managers.UI.MakeSubItem<UI_Inven_Item>(gridPanel.transform).gameObject;

            UI_Inven_Item invenitem = item.GetOrAddComponent<UI_Inven_Item>();
            invenitem.SetInfo($"집행검{i}번");
        }
    }
    
}
