using RISC_V_Pipline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        int dataHazards = 0;
        int controlHazards = 0;
        int structuralHazard = 0;

        public Dictionary<string, int> SymbolTable = new Dictionary<String, int>();

        public Instruction F_DEC { get; set; }
        public Instruction DEC_EX { get; set; }
        public Instruction EX_MEM { get; set; }
        public Instruction MEM_WB { get; set; }

        public Pipeline(string instructionSet)
        {
            instructions = instructionSet.Split('\n');
            for(int i = 0; i < registers.Length; i++)
                registers[i] = false;

            for(int i = 0; i < instructions.Length; i++)
            {
                if (!instructions[i].Contains(' '))
                    SymbolTable.Add(instructions[i], i);
            }
                
        }

        int Fetch()
        {
            string instructionString = instructions[programCounter++];
            instructionString.Replace(',', ' ');

            string[] instruction = instructionString.Split(' ');

            return Decode(instruction) + 1;
        }

        int Decode(string[] instruction)
        {
            Instruction i = new Instruction();

            if (instruction[0] == "sw")
            {
                i.Operand = InstructionType.SW;
                i.Destination = instruction[1];
                i.Source1 = instruction[2];
            }
            else if (instruction[0] == "lw")
            {
                i.Operand = InstructionType.LW;
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
                i.ImmediateFlag = false;

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

            return 1 + Execute(i);
        }

        int Execute(Instruction instruction)
        {
            if(instruction.Operand == InstructionType.ARITHMETIC && !instruction.ImmediateFlag)
            {
                int source1 = Convert.ToInt32(instruction.Source1.Remove('x'));
                int source2 = Convert.ToInt32(instruction.Source2.Remove('x'));
                int destination = Convert.ToInt32(instruction.Destination.Remove('x'));

                if (!registers[source1] || !registers[source2])
                {
                    registers[destination] = true;
                    dataHazards++;
                    return 2 + WriteBack();
                }

                registers[destination] = true;
            }
            else if(instruction.Operand == InstructionType.ARITHMETIC && instruction.ImmediateFlag)
            {
                int source = Convert.ToInt32(instruction.Source1.Remove('x'));
                int destination = Convert.ToInt32((instruction.Destination.Remove('x')));

                if (!registers[source])
                {
                    registers[destination] = true;
                    dataHazards++;
                    return 2 + WriteBack();
                }

                registers[destination] = true;
            }
            else if(instruction.Operand == InstructionType.LW)
            {
                int destination = Convert.ToInt32(instruction.Destination.Remove('x'));
                registers[destination] = false;
                return 1 + Memory(instruction);
            }
            else if(instruction.Operand == InstructionType.SW)
            {
                int source = Convert.ToInt32(instruction.Destination.Remove('x'));

                return 1 + Memory(instruction);
                
            }
            else if(instruction.Operand == InstructionType.EXECUTE)
            {
                string label = instruction.Label;
                programCounter = SymbolTable[label];

            }

            return 1 + WriteBack();
        }

        int Memory(Instruction instruction)
        {
            if(instruction.Operand == InstructionType.LW || instruction.Operand == InstructionType.SW)
            {
                int destination = Convert.ToInt32(instruction.Destination.Remove('x'));
                registers[destination] = true;
            }
            
            return 3 + WriteBack();
        }

        int WriteBack()
        {
            return 1;
        }
    }
}

