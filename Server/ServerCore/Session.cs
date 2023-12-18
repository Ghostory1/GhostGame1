using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;

        object _lock = new object();
        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        public void Start(Socket socket)
        {
            _socket = socket;
            
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            recvArgs.SetBuffer(new byte[1024],0,1024); //0번째부터 사용하겠다.
            

            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Send(byte[] sendBuff)
        {
            lock(_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                {
                    //첫번째로 Send를 호출해서 전송까지 할 수 있다면
                    RegisterSend();
                }
            }
            
        }
        public void Disconnect()
        {
            // Disconnected를 2번 호출해서 오류나는걸 방지
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;
            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            
            
        }

        #region 네트워크 통신
        void RegisterSend()
        {
            

            //byte[] buff = _sendQueue.Dequeue();
            //_sendArgs.SetBuffer(buff, 0, buff.Length);
            
            while(true)
            {
                byte[] buff = _sendQueue.Dequeue();
                //ArraySegment는 F12로 타고가보면 구조체로 선언되어있어서 Stack 영역에 할당됌 그래서 Add할땐 값이 복사 돼서 처리
                _pendingList.Add(new ArraySegment<byte>(buff,0,buff.Length) );
            }
            _sendArgs.BufferList = _pendingList;

            bool pending = _socket.SendAsync(_sendArgs);
            if (pending == false)
            {
                OnSendCompleted(null, _sendArgs);
            }
        }
        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        OnSend(_sendArgs.BytesTransferred);
                        Console.WriteLine($"Transferred bytes : {_sendArgs.BytesTransferred}");
                        if (_sendQueue.Count >0)
                        {
                            RegisterSend();
                        }
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed{e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
            
        }

        void RegisterRecv()
        {
            bool pending = _socket.ReceiveAsync(recvArgs);
            if(pending == false)
            {
                //false인건 운좋게 받을 데이터가 바로 있어서 데이터를 받아줌
                OnRecvCompleted(null, recvArgs);
            }
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            //가끔 연결을 끊었을때 0바이트가 오기때문에 걸러 줘야한다.
            if(args.BytesTransferred >0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));
                    //args.Offset -> 어디서 부터 시작할지
                    //args.BytesTransferred -> 몇바이트를 받았는지
                    
                    RegisterRecv();
                }
                catch(Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed{e}");
                }
            }
            else
            {
                //TODO Disconnect
                Disconnect();
            }
        }
        #endregion
    }
}
