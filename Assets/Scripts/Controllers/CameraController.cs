using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Define.CameraMode _mode = Define.CameraMode.QuarterView;
    
    [SerializeField]
    Vector3 _delta = new Vector3(0.0f, 6.0f, -5.0f); //플레이어 기준으로 카메라랑 떨어진 거리의 방향 벡터 _delta
    [SerializeField]
    GameObject _player = null;
    void Start()
    {
        
    }

    
    void LateUpdate()
    {
        if (_mode == Define.CameraMode.QuarterView)
        {
            RaycastHit hit;
            if(Physics.Raycast(_player.transform.position , _delta , out hit , _delta.magnitude , LayerMask.GetMask("Wall")))
            {
                //충돌한 벽 - 플레이어 좌표 의 방향벡터의 크기
                float dist = (hit.point - _player.transform.position).magnitude * 0.8f; //방향벡터의 크기 보다 0.8 앞으로 덍겨줌
                transform.position = _player.transform.position + _delta.normalized * dist;
            }
            else
            {
                transform.position = _player.transform.position + _delta;
                transform.LookAt(_player.transform);
            }
            
        }
    }

    public void SetQuaterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        _delta = delta;
    }
}


