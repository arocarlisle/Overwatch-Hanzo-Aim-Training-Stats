using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Overwatch_Aim_Traning
{
    public class Hero
    {
        public string name;
        public int fireDelay;
        public bool weaponReady = true;

        public Hero(string name, int fireDelay)
        {
            this.name = name;
            this.fireDelay = fireDelay;
        }
    }
}
