using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP_lab1
{
    class Fork
    {
        private bool isClean = false;
        private bool hasFork = false;
        public bool IsClean { get => isClean; set => isClean = value; }
        public bool HasFork { get => hasFork; set => hasFork = value; }

        public Fork(bool isClean, bool hasFork)
        {
            this.isClean = isClean;
            this.hasFork = hasFork;
        }
    }
}
