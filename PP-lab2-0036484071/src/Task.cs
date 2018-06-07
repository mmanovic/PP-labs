using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PP_lab2
{
    [Serializable]
    public class Task
    {
        public Board b;
        public int nextPlayer;
        public string key;
        public double value;

        public Task(Board b, int nextPlayer, string key)
        {
            this.b = b;
            this.nextPlayer = nextPlayer;
            this.key = key;
        }

        public override string ToString()
        {
            return key;
        }
    }
}
