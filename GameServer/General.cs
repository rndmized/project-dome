using System;

namespace GameServer
{
    class General
    {
        public void InitServer()
        {
            //Globals.mysql.MySQLInit();
            InitGameData();
            Globals.network.InitTCP();
        }

        public void InitGameData()
        {
            for (int i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                Globals.Clients[i] = new Client();
            }
        }
    }
}
