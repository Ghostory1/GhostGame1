using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class SendBufferHelper
    {
        //전역으로 선언하면 SendBuffer은 처음 할당한 크기에서 사용이 가능하면 재사용을 할 건데
        //멀티 쓰레드환경에서는 경합이 될것이다.
        //그래서 이전에 배운 전역이지만 나의 쓰레드에서 고유하게 사용가능한 전역 선언방법
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });

        public static int ChunkSize { get; set; } = 4096 * 100;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (CurrentBuffer.Value == null)
            {
                //null이면 한번도 사용하지 않은 상태이니 생성
                CurrentBuffer.Value = new SendBuffer(ChunkSize);
            }

            if (CurrentBuffer.Value.FreeSize < reserveSize)
            {
                //지금 남은 공간이 예약공간보다 적으면 새로운 버퍼로 갈아끼움
                CurrentBuffer.Value = new SendBuffer(ChunkSize);
            }

            //여기까지 조건문을 통과했으면 남은공간이 있다는거니까 Open
            return CurrentBuffer.Value.Open(reserveSize);
        }
        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }


    }

    public class SendBuffer
    {
        //[] [] [] [u] [] [] [] [] [] []
        byte[] _buffer;
        int _usedSize = 0;

        //남은 공간
        public int FreeSize { get { return _buffer.Length - _usedSize; } }

        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];
        }

        public ArraySegment<byte> Open(int reserveSize)
        {
            //최대 할당 크기
            //남은공간이 남아있으면 reserveSize만큼 할당해줌
            //예약공간(reserveSize)보다 남은 공간(FreeSize)이 적으면 buffer가 고갈된것이니
            //사용할수없으니 null을 리턴

            if (reserveSize > FreeSize)
                return ArraySegment<byte>.Empty;

            return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
        }

        public ArraySegment<byte> Close(int usedSize)
        {
            //실제 내가 사용한 사이즈
            //( 위의 Open에서 3바이트를 할당했지만 실제로는 2바이트만 사용했으면 남은 공간을 반환)
            //유효범위 리턴
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;
            return segment;
        }
    }
}
