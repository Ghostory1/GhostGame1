using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//1.위치 벡터
//2.방향 벡터

public class PlayerController : MonoBehaviour
{
    //굳이 public으로 할 필요는 없으니 리플렉션으로 처리 SerializeField
    [SerializeField]
    float _speed = 10.0f;

    //bool _moveToDest = false; 키보드 이동할때 사용했던 변수
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
        //아무것도 못함
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

        //애니메이션
        Animator anim = GetComponent<Animator>();
        //현재 게임 상태에 대한 정보를 넘겨준다.
        anim.SetFloat("speed", _speed);
        

    }

    void UpdateIdle()
    {
        //애니메이션
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
    //    //transform.Translate -> 자기가 바라보고있는 방향의 로컬좌표를 기준으로 연산

    //    //transform.position += transform.TransformDirection    로컬 -> 월드 좌표계 기준으로 연산

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
            //_moveToDest = true; 키보드이동에 사용했던 변수
            
            
            //hit.collider.gameObject.tag;
            //Debug.Log($"Raycast Camera @ {hit.collider.gameObject.name}");
        }
    }
}
