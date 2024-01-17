using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{

    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected !{endPoint}");

            C_PlayerInfoReq packet = new C_PlayerInfoReq() {  playerId = 1001 , name = "ABCD"};

            var skill = new C_PlayerInfoReq.Skill() { id = 101, level = 1, duration = 3.0f };
            skill.attributes.Add(new C_PlayerInfoReq.Skill.Attribute() { att = 77 });
            packet.skills.Add(skill);

            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 201, level = 2, duration = 4.0f });
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 301, level = 3, duration = 5.0f });
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 401, level = 4, duration = 6.0f });

            //보낸다
            //for (int i = 0; i < 5; i++)
            {
                //1.패킷 클래스를 만들어서 Write 를 해서 serialize를 해주고
                //2.byte배열로 만들어준 다음에 보내준다
                ArraySegment<byte> s =packet.Write();
                
                if (s != null)
                    Send(s);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected !{endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");

            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
