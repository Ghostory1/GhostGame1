using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ServerCore
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
            byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
            Send(sendBuff);

            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
    class Program 
    {
        
        static Listener _listenser = new Listener();
        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                GameSession session = new GameSession();
                session.Start(clientSocket);
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
                session.Send(sendBuff);

                Thread.Sleep(1000);
                session.Disconnect();
                
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.ToString());
            }
            
        }

        static void Main(string[] args)
        {
            //listSocket 인자
            //DNS(Domain Name System)
            // www.google.com 의 도메인을 설정 후 -> 123.123.123.12 같은 ip를 등록해서 관리하면 나중에 ip를 바꾼다거나 할때 관리가 용이해진다.
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr,7777); //아까 식당에 비유하면 인자로 넣어준 ipAddr가 식당주소, 7777는 포트번호로 정문인지 후문인지 식당의 문을 나타냄

            _listenser.Init(endPoint,() => { return new GameSession(); });
            
            Console.WriteLine("Listening.....");
            while (true)
            {
                ;
            }

        }

    }
}