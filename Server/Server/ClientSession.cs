using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
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

        

        //    public override void Read(ArraySegment<byte> s)
        //    {
        //        ushort count = 0;

        //        //ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
        //        count += 2;
        //        //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
        //        count += 2;
        //        this.playerId = BitConverter.ToInt64(s.Array, s.Offset + count);
        //        count += 8;
        //    }

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

    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected !{endPoint}");

            //Packet packet = new Packet() { size = 100, packetId = 10 };
            //
            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);
            //
            //Send(sendBuff);
            Thread.Sleep(5000);
            Disconnect();
        }
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;
            switch((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        long playerId = BitConverter.ToInt64(buffer.Array, buffer.Offset + count);
                        count += 8;
                        Console.WriteLine($"PlayerInfoReq: {playerId} ");
                    }
                    break;
            }

            Console.WriteLine($"RecvPacketId: {id} , Size {size}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected !{endPoint}");
        }



        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
