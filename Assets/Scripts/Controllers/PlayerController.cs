using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//1.��ġ ����
//2.���� ����

public class PlayerController : MonoBehaviour
{
    //���� public���� �� �ʿ�� ������ ���÷������� ó�� SerializeField
    [SerializeField]
    float _speed = 10.0f;

    //bool _moveToDest = false; Ű���� �̵��Ҷ� ����ߴ� ����
    Vector3 _destPos;
    
    void Start()
    {
        //Managers.Input.KeyAction -= OnKeyboard;
        //Managers.Input.KeyAction += OnKeyboard;
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;

    }

    // GameObject (Player)
    // Transform
    //PlayerController(*)

    //float _yAngle = 0.0f;
    //float wait_run_ratio = 0.0f;

    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
        //Channeling,
        //Jumping,
        //Falling,
    }

    
    PlayerState _state = PlayerState.Idle;

    void UpdateDie()
    {
        //�ƹ��͵� ����
    }
    void UpdateMoving()
    {
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.0001f)
        {
            _state = PlayerState.Idle;
        }
        else
        {
            float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude);
            transform.position = transform.position + dir.normalized * moveDist;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
            //transform.LookAt(_destPos);
        }

        //�ִϸ��̼�
        Animator anim = GetComponent<Animator>();
        //���� ���� ���¿� ���� ������ �Ѱ��ش�.
        anim.SetFloat("speed", _speed);
        

    }

    void UpdateIdle()
    {
        //�ִϸ��̼�
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", 0);

    }
    void Update()
    {
        switch (_state)
        {
            case PlayerState.Die:
                UpdateDie();
                break;
            case PlayerState.Moving:
                UpdateMoving();
                break;
            case PlayerState.Idle:
                UpdateIdle();
                break;
        }
    }

    //void OnKeyboard()
    //{
    //    //transform.Translate -> �ڱⰡ �ٶ󺸰��ִ� ������ ������ǥ�� �������� ����

    //    //transform.position += transform.TransformDirection    ���� -> ���� ��ǥ�� �������� ����

    //    if (Input.GetKey(KeyCode.W))
    //    {
    //        //transform.rotation = Quaternion.LookRotation(Vector3.forward);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
    //        //transform.Translate(Vector3.forward * Time.deltaTime * _speed);
    //        transform.position += transform.TransformDirection(Vector3.forward * Time.deltaTime * _speed);
    //    }
    //    if (Input.GetKey(KeyCode.S))
    //    {
    //        //transform.rotation = Quaternion.LookRotation(Vector3.back);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
    //        //transform.Translate(Vector3.forward * Time.deltaTime * _speed);
    //        transform.position += transform.TransformDirection(Vector3.forward * Time.deltaTime * _speed);
    //    }
    //    if (Input.GetKey(KeyCode.A))
    //    {
    //        //transform.rotation = Quaternion.LookRotation(Vector3.left);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
    //        //transform.Translate(Vector3.forward * Time.deltaTime * _speed);
    //        transform.position += transform.TransformDirection(Vector3.forward * Time.deltaTime * _speed);
    //    }
    //    if (Input.GetKey(KeyCode.D))
    //    {
    //        //transform.rotation = Quaternion.LookRotation(Vector3.right);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
    //        //transform.Translate(Vector3.forward * Time.deltaTime * _speed);
    //        transform.position += transform.TransformDirection(Vector3.forward * Time.deltaTime * _speed);
    //    }

    //    _moveToDest = false;
    //}
    
    void OnMouseClicked(Define.MouseEvent evt)
    {
        //if (evt != Define.MouseEvent.Click)
        //    return;

        if (_state == PlayerState.Die)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        
        //LayerMask mask = LayerMask.GetMask("Monster") | LayerMask.GetMask("Wall");
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Wall")))
        {
            _destPos = hit.point;
            _state = PlayerState.Moving;
            //_moveToDest = true; Ű�����̵��� ����ߴ� ����
            
            
            //hit.collider.gameObject.tag;
            //Debug.Log($"Raycast Camera @ {hit.collider.gameObject.name}");
        }
    }
}
