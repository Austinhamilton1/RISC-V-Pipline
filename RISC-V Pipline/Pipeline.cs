using RISC_V_Pipline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RISC_V_Pipeline
{
    public class Pipeline
    {
        bool[] registers = new bool[32];

        string[] instructions;

        int programCounter = 0;

        bool errorFlag = false;

        public Pipeline(string instructionSet)
        {
            instructions = instructionSet.Split('\n');
        }

        void Fetch()
        {
            string instructionString = instructions[programCounter++];
            instructionString.Replace(',', ' ');

            string[] instruction = instructionString.Split(' ');

            Decode(instruction);
        }

        void Decode(string[] instruction)
        {
            Instruction i = new Instruction();

            if (instruction[0] == "sw" || instruction[0] == "lw")
            {
                i.Operand = InstructionType.MEMORY;
                i.Destination = instruction[1];
                i.Source1 = instruction[2];
            }
            else if (instruction[0] == "add" || instruction[0] == "sub"
                || instruction[0] == "addi")
            {
                i.Operand = InstructionType.ARITHMETIC;
                i.Destination = instruction[1];
                i.Source1 = instruction[2];
                i.Source2 = instruction[3];

                if (instruction[0] == "addi")
                    i.ImmediateFlag = true;
            }
            else if (instruction[0] == "bne" || instruction[0] == "beq")
            {
                i.Operand = InstructionType.EXECUTE;
                i.Destination = instruction[1];
                i.Source1 = instruction[2];
                i.Label = instruction[3];
            }
            else
                errorFlag = true;

            Execute(i);
        }

        void Execute(Instruction instruction)
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

