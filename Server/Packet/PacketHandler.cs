using Server;
using Server.Session;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class PacketHandler
{
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        C_Chat chatPacket = packet as C_Chat;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        //바로 BroadCast를 하는게 아니고 행동을 JobQueue에 넣어준다.
        //람다식으로 처리하고 있는데 우리가 바로 처리하는게 아니고
        //JobQueue에 예약 한다음 처리를 해주는데
        //참조할 Room이 중간에 사라져버리면 에러가 나니 새로운 룸으로 참조하는 식으로 크래시 방지
        GameRoom room = clientSession.Room;
        room.Push(() => room.BroadCast(clientSession, chatPacket.chat));
        
    }

}

