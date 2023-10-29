using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Popup
{
    enum Buttons
    {
        PointButton
    }

    enum Texts 
    {
        PointText,
        ScoreText,
    }

    enum GameObjects
    {
        TestObject,
    }

    enum Images
    {
        ItemIcon,
    }

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        //부모의 Init도 호출
        base.Init();

        //리플렉션으로 enum을 넘겨주는 법 typeof
        Bind<Button>(typeof(Buttons)); //enum은 Buttons인데 여기다가 Button 이라는 컴포넌트를 찾아서 맵핑해달라는 뜻
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));


        Get<Button>((int)Buttons.PointButton).gameObject.BindEvent(onButtonClicked); //익스텐션으로 구현 GameObject에 함수자체를 넣어줌

        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        BindEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag);

    }

    int _score = 0;
    //주의 : public으로 안해주면 UI에서 안뜸
    public void onButtonClicked(PointerEventData data)
    {
        _score++;
        Get<Text>((int)Texts.ScoreText).text = $"점수 :{_score}";

    }
    
}
