using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace RISC_V_Pipline
{

    internal class Instruction
    {
        private InstructionType operand { get; set; }
        private string reg1;
        private string reg2;
        private int offset;

        public Instruction(InstructionType _operand, string reg1, string reg2, int offset)
        {
            this.operand = _operand;
            this.reg1 = reg1;
            this.reg2 = reg2;
            this.offset = offset;
        }
    }

    internal enum InstructionType
    {
        MEMORY, ARITHMETIC, EXECUTE
    }

}

