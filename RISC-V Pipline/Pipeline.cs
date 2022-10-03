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

        int dataHazards = 0;
        int controlHazards = 0;
        int structuralHazard = 0;

        int[] lastOp = { 0, 0, 0, 0 };

        List<int[]> cycles = new List<int[]>();

        public Instruction F_DEC { get; set; }
        public Instruction DEC_EX { get; set; }
        public Instruction EX_MEM { get; set; }
        public Instruction MEM_WB { get; set; }

        public Pipeline(string instructionSet)
        {
            instructions = instructionSet.Split('\n');
            for(int i = 0; i < registers.Length; i++)
                registers[i] = false;
                
        }

        public void RunPipeline()
        {
            for (int i = 0; i < instructions.Length; i++)
                Fetch();
        }

        void Fetch()
        {
            cycles.Add(new int[5]);

            string instructionString = instructions[programCounter++];
            instructionString = instructionString.Replace(",", "");

            string[] instruction = instructionString.Split(' ');

            cycles[programCounter - 1][0] = Math.Max(1, lastOp[0]);

            Decode(instruction, cycles[programCounter - 1]);
        }

        void Decode(string[] instruction, int[] cycle)
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
                i.Operand = InstructionType.CONTROL;
                i.Destination = instruction[1];
                i.Source1 = instruction[2];
                i.Label = instruction[3];
            }

            cycle[1] = Math.Max(1, lastOp[1]);

            if(1 > lastOp[0])
                lastOp[0] = 1;

            Execute(i, cycle);
        }

        void Execute(Instruction instruction, int[] cycle)
        {
            if(instruction.Operand == InstructionType.ARITHMETIC && !instruction.ImmediateFlag)
            {
                int source1 = Convert.ToInt32(instruction.Source1.Substring(1));
                int source2 = Convert.ToInt32(instruction.Source2.Substring(1));
                int destination = Convert.ToInt32(instruction.Destination.Substring(1));

                if (!registers[source1] || !registers[source2])
                {
                    registers[destination] = true;
                    dataHazards++;
                    cycle[2] = Math.Max(1, lastOp[2]);
                    
                    if(1 > lastOp[1])
                        lastOp[1] = 1;

                    Memory(instruction, cycle, false);
                }

                registers[destination] = true;
            }
            else if(instruction.Operand == InstructionType.ARITHMETIC && instruction.ImmediateFlag)
            {
                int source = Convert.ToInt32(instruction.Source1.Substring(1));
                int destination = Convert.ToInt32(instruction.Destination.Substring(1));

                if (!registers[source])
                {
                    registers[destination] = true;
                    dataHazards++;
                    cycle[2] = Math.Max(1, lastOp[2]);

                    if (1 > lastOp[1])
                        lastOp[1] = 2;

                    Memory(instruction, cycle, false);
                }

                registers[destination] = true;
            }
            else if(instruction.Operand == InstructionType.LW)
            {
                int destination = Convert.ToInt32(instruction.Destination.Substring(1));
                registers[destination] = false;
                
                cycle[2] = Math.Max(1, lastOp[2]);

                if (1 > lastOp[1])
                    lastOp[1] = 1;

                Memory(instruction, cycle, true);
            }
            else if(instruction.Operand == InstructionType.SW)
            {
                int source = Convert.ToInt32(instruction.Destination.Substring(1));

                cycle[2] = Math.Max(1, lastOp[2]);

                if (1 > lastOp[1])
                    lastOp[1] = 1;

                Memory(instruction, cycle, true);

            }
            else if(instruction.Operand == InstructionType.CONTROL)
            {
                cycle[2] = Math.Max(2, lastOp[2]);
                controlHazards++;

                Memory(instruction, cycle, false);
            }
        }

        void Memory(Instruction instruction, int[] cycle, bool access)
        {
            if (access)
            {
                int destination = Convert.ToInt32(instruction.Destination.Substring(1));
                registers[destination] = true;

                cycle[3] = Math.Max(3, lastOp[3]);

                if (3 > lastOp[2])
                    lastOp[2] = 3;

                WriteBack(cycle);
            }
            else
            {
                cycle[3] = Math.Max(1, lastOp[3]);

                if (1 > lastOp[2])
                    lastOp[2] = 1;

                WriteBack(cycle);
            }
        }

        void WriteBack(int[] cycle)
        {
            cycle[4] = 1;

            for (int i = 0; i < lastOp.Length - 1; i++)
                lastOp[i] = lastOp[i + 1];

            lastOp[3] = 1;
        }

        public void PrintList()
        {
            foreach (int[] row in cycles)
            {
                foreach (int cycle in row)
                {
                    Console.Write($"\t{cycle}");
                }

                Console.WriteLine();
            }
        }
    }
}

