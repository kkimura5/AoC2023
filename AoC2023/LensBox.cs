using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    internal class LensBox
    {
        public LensBox(int boxNumber)
        {
            BoxNumber = boxNumber;
        }

        public int BoxNumber { get; }
        public List<Lens> Lenses { get; set; } = new List<Lens>();
    }
}
