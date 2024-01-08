using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {   //   0  1  2  3  4  5  6  7  8  9
        // [rw] [] [] [] [] [] [] [] [] []
        ArraySegment<byte> _buffer;
        int _readPos;
        int _writePos;

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize { get { return _writePos - _readPos; } }
        public int FreeSize { get { return _buffer.Count - _writePos; } }


        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }
        public ArraySegment<byte> WriteSegment
        {
            //다음에 Receive할때 어디서부터 어디까지가 유효범위인지 남는 범위
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        public void Clean()
        {
            //중간중간 정리해서 처음으로 버퍼 되돌리기
            //why? 2바이트씩 처리한다하면 3바이트를 받았을때 한바이트씩 뒤로 밀리게 되니까 계속 밀리다보면 나중에 FreeSize가 없어지니 중간중간에 한번씩 처음으로 되돌림
            int dataSize = DataSize;
            if (dataSize == 0)
            {
                //클라에서 보내준 데이터를 모두 처리한 상태
                //남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
                _readPos = _writePos = 0;
            }
            else
            {
                //남은 데이터가 있다.
                //시작위치로 복사
                //[] [] [r] [] [w] [] [] [] [] []
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
                //이렇게 옮겨준 작업
                //[r] [] [w] [] [] [] [] [] [] []
            }
        }

        public bool OnRead(int numOfBytes)
        {
            //유효범위를 초과한 경우
            if (numOfBytes > DataSize)
                return false;

            _readPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            //유효범위를 초과한 경우
            if (numOfBytes > FreeSize)
                return false;

            _writePos += numOfBytes;
            return true;
        }
    }
}
