using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Lib
{
    public class Quest 
    {
        private string Task { get; init; }
        private string Difficulty { get; init; }
        private int RewardAmmount { get; init; }
        private string RewardName { get; init; }
        internal int Ammount { get; init; }
        internal int Done { get; set; }
        
        public Quest(string task,string difficulty,string reward,int ammount)
        {
            string[] reward = reward.Trim(" ").Split(" ");
            Task = task;
            Ammount = ammount;
            Difficulty = difficulty;
            RewardAmmount = reward[0];
            RewardName = reward[1];
        }
        
    }
}
