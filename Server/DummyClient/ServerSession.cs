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
    public class Packet //패킷 헤더
    {
        public ushort size;
        public ushort packetId;

        //public abstract ArraySegment<byte> Write();
        //public abstract void Read(ArraySegment<byte> s);
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;

        

        //public override void Read(ArraySegment<byte> s)
        //{
        //    ushort count = 0;

        //    //ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
        //    count += 2;
        //    //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
        //    count += 2;
        //    this.playerId = BitConverter.ToInt64(s.Array, s.Offset + count);
        //    count += 8;
        //}eeeeeeee

        //public override ArraySegment<byte> Write()
        //{
        //    ArraySegment<byte> s = SendBufferHelper.Open(4096);

        //    ushort count = 0;
        //    bool success = true;
        //    // &= (and연산) 중간에 한번이라도 계산에 실패하면 false를 반환
        //    // 그럼 TryWriteBytes가 실패하는 경우는 ? 
        //    // 애당초 2바이트짜리 크기인데 넣어주는 packet.size의 부분이 8바이트 짜리면 공간이 모자라서 실패
        //    //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), packet.size);
        //    count += 2;
        //    success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId);
        //    count += 2;
        //    success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);

        //    count += 8;
        //    //우리가 pakcet.size는 다 추가한 후에서야 알 수있으니 마지막에 추가
        //    success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

        //    if (success == false)
        //        return null;

        //    //ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);
        //    return SendBufferHelper.Close(count);

        //}
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }

    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected !{endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { size = 4, packetId = (ushort)PacketID.PlayerInfoReq, playerId = 1001 };

            //보낸다
            //for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> s = SendBufferHelper.Open(4096);

                //byte[] = new byte[4]; 뭐 이런식으로 할당해서 다시 넣어주는거라 부담이 될 수있다. 
                // 위에 우리가 선언한 s 배열에 바로 넣는 방법은 없을까 ?
                //byte[] size = BitConverter.GetBytes(packet.size);
                //byte[] pakedId = BitConverter.GetBytes(packet.packetId);
                //byte[] playerId = BitConverter.GetBytes(packet.playerId);
                //Array.Copy(size, 0, s.Array, s.Offset + count, 2);
                //count += 2;
                //Array.Copy(pakedId, 0, s.Array, s.Offset + count, 2);
                //count += 2;
                //Array.Copy(playerId, 0, s.Array, s.Offset + count, 8);
                //count += 8;
                //byte[] 이후 Array.Copy하는 방식은 안정성이 좋지만 속도가 안좋다. 밑에 방식을 써보자

                //BitConverter.TryWriteBytes
                bool success = true;
                ushort count = 0;
                //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), packet.size);
                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count ), packet.packetId);
                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.playerId);
                count += 8;
                //size는 마지막에 count로 배열 처음에 넣어준다.
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);
                ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);
                if(success)
                    Send(sendBuff);
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
