using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace RISC_V_Pipeline
{
    public class Pipeline
    {
        bool[] registers = new bool[32];

        string[] instructions;

        int programCounter = 0;

        public Pipeline(string instructionSet)
        {
            instructions = instructionSet.Split('\n');
        }

        void Fetch()
        {
            

        }

        void Decode()
        {

        }

        void Execute()
        {

        }

        void Memory()
        {

        }

        void WriteBack()
        {

        }
    }
}

