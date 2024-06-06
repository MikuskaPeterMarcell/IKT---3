using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Game_Lib
{
    public class Quests
    {
        public int SmallEnemyDead { get; set; }
        public int LargeEnemyDead { get; set; }
        public int LevelReached { get; set; }
        public int PointCollected { get; set; }
        public int TaskComplete { get; set; }

        List<Quest> tasks = new();

        public Quests(string[] file)
        {
            foreach (var item in file)
            {
                string[] line = item.Split(";");
                tasks.Add(new Quest(line[0], line[1], line[2], int.Parse(line[3])));
            }
            SmallEnemyDead = 0;
            LargeEnemyDead = 0;
            PointCollected = 0;
            TaskComplete = 0;
        }


        public string CheckTasks()
        {
            if (SmallEnemyDead > 5)
            {
                return "Golden Sword";
            }
            else
            {
                return "Not Done";
            }
        }
    }
}
