using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log($"Collision @ {collision.gameObject.name}");
    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log($"Trigger !@ {other.gameObject.name}");
    //}

    void Start()
    {
        
    }

    
    void Update()
    {
        //Local -> World -> Viewport -> Screen

        //스크린좌표 표시
        //Debug.Log(Input.mousePosition);

        //Viewport
        //Debug.Log(Camera.main.ScreenToViewportPoint(Input.mousePosition));

        //Wolrd 좌표계
        //if (Input.GetMouseButton(0))
        //{
        //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        //    Vector3 dir = mousePos - Camera.main.transform.position;
        //    dir = dir.normalized;

        //    Debug.DrawRay(Camera.main.transform.position, dir * 100.0f, Color.red, 1.0f);

        //    RaycastHit hit;
        //    if (Physics.Raycast(Camera.main.transform.position, dir, out hit,100.0f))
        //    {
        //        Debug.Log($"Raycast Camera @ {hit.collider.gameObject.name}");
        //    }
        //}

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

            //쉬프트 연산 8번 비트만 키고 나머지 0으로 밀기 
            //int mask = (1 << 8) | (1 << 9);
            LayerMask mask = LayerMask.GetMask("Monster") | LayerMask.GetMask("Wall");
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit,100.0f, mask))
            {
                //hit.collider.gameObject.tag;
                Debug.Log($"Raycast Camera @ {hit.collider.gameObject.name}");
            }
        }



        //Vector3 look = transform.TransformDirection(Vector3.forward);
        //Debug.DrawRay(transform.position+Vector3.up, look * 10,Color.red);
        //RaycastHit hit;
        //RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up, look, 10);

            //foreach (RaycastHit hit1 in hits)
            //{
            //    Debug.Log($"Raycast ! {hit1.collider.gameObject.name}");
            //}



            //if(Physics.Raycast(transform.position+ Vector3.up, look, out hit,10))
            //{
            //    Debug.Log($"Raycast ! {hit.collider.gameObject.name}");
            //}
    }
}
