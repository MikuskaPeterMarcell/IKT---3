using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Lib
{
    public class Quests
    {
        List<Quest> quests = new();

        public Quests(string[] file)
        {
            foreach (var item in file)
            {
                string[] line = item.Split(";");
                quests.add(new quests(line[0], line[1], line[2], int.Parse(line[3])));
            }
        }
        //public void SmallEnemyDead
    }
}
