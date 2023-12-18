using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class Listener
    {
        //1.문지기의 휴대폰 listSocket을 만들어준다.
        //endPoint.AddressFamily -> IP버전4 , IP버전6 를 사용할건지 넣어주는건데 위에 Dns서버에서 사용한 endPoint의 Address를 넣어줌
        //그리고 TCP를 사용할지 UDP를 사용할지 선택인데 TCP로 설정해보면 밑에처럼 인자를 넣어주면 된다. 소켓타입을 Stream 으로 프로토콜타입을 Tcp로 
        Socket _listenSocket;
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            //2.문지기 교육
            _listenSocket.Bind(endPoint);

            //3.영업 시작
            //backlog인자로 최대 대기수를 받고 있다.
            _listenSocket.Listen(10);

            //한번만 만들어두면 계속 재사용할수있는 장점이 있다.
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args);
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            //OnAcceptCompleted 가 실행되면 2번째 RegisterAccept부터는 args에는 값이 들어가있는걸 계속 Register과 Completed가 반복되니 계속 비워줘야한다.
            args.AcceptSocket = null;
            bool pending = _listenSocket.AcceptAsync(args);
            if(pending ==false)
            {
                //peding이 false일때만 호출 -> false일때는 언제인가 ? 클라이언트가 Connect를 완료했을 때 입장 가능한 상태
                OnAcceptCompleted(null,args);
            }
        }

        void OnAcceptCompleted(object sender,SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                //에러 없이 잘 처리 됌
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
               
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            RegisterAccept(args);
        }

        public Socket Accept()
        {
            //그냥 Accept는 블로킹 형태의 함수
            //AcceptAsync는 비동기 함수 즉, 바로 동시에 처리 되지않고 조금 나중에 처리될 수 있는 함수
            
            return _listenSocket.Accept();
        }
    }
}
