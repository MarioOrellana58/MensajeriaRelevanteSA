using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlternativeProcesses.CompressComponents
{
    public class NodeComponents
    {
        public initialReadComponents CharacterData { get; set; }
        public NodeComponents LeftSon { get; set; }
        public NodeComponents RigthSon { get; set; }
    }
}
