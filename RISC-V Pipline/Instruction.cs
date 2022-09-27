using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace RISC_V_Pipline
{

    internal class Instruction
    {
        public InstructionType operand { get; set; }
        public string reg1 { get; set; }
        public string reg2 { get; set; }

        public Instruction(InstructionType _operand, string reg1, string reg2)
        {
            this.operand = _operand;
            this.reg1 = reg1;
            this.reg2 = reg2;

        }
    }

    internal enum InstructionType
    {
        MEMORY, ARITHMETIC, EXECUTE
    }

}

