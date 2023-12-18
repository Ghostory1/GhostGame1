using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //DNS(Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); //아까 식당에 비유하면 인자로 넣어준 ipAddr가 식당주소, 7777는 포트번호로 정문인지 후문인지 식당의 문을 나타냄

            while(true)
            {
                //클라이언트 입장
                //1. 휴대폰 설정
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    //2. 문지기한테 입장 문의
                    //게임 개발에선 이런 Connect 같은 블로킹 계열을 쓰면 안된다. 서버가 안받아주면 계속 기다려야 할 수도있기 때문이다.
                    socket.Connect(endPoint);
                    Console.WriteLine($"Connect To {socket.RemoteEndPoint.ToString()}");

                    //보낸다
                    for(int i=0; i<5; i++)
                    {
                        byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World!{i}");
                        //Send도 블로킹처럼 서버가 안받아주면 계속 기다릴수도있다.
                        int sendBytes = socket.Send(sendBuff);
                    }

                    //받는다
                    byte[] recvBuff = new byte[1024];
                    //Receive도 블로킹처럼 서버가 안받아주면 계속 기다릴수도있다.
                    int recvBytes = socket.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From Server] {recvData}");

                    //나간다
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            
            Thread.Sleep(100);
        }
    }
}