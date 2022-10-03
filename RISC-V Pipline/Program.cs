namespace RISC_V_Pipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;
            string instructions = string.Empty;

            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
                instructions += line + '\n';

            Pipeline p = new Pipeline(instructions);

            p.RunPipeline();

            p.PrintList();
        }
    }
}
