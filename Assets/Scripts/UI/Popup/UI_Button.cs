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
        //�θ��� Init�� ȣ��
        base.Init();

        //���÷������� enum�� �Ѱ��ִ� �� typeof
        Bind<Button>(typeof(Buttons)); //enum�� Buttons�ε� ����ٰ� Button �̶�� ������Ʈ�� ã�Ƽ� �����ش޶�� ��
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));


        Get<Button>((int)Buttons.PointButton).gameObject.BindEvent(onButtonClicked); //�ͽ��ټ����� ���� GameObject�� �Լ���ü�� �־���

        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        BindEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag);

    }

    int _score = 0;
    //���� : public���� �����ָ� UI���� �ȶ�
    public void onButtonClicked(PointerEventData data)
    {
        _score++;
        Get<Text>((int)Texts.ScoreText).text = $"���� :{_score}";

    }
    
}
