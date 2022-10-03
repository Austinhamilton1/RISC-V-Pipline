namespace RISC_V_Pipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            string instructions = Console.ReadLine();

            Pipeline p = new Pipeline(instructions);

            p.RunPipeline();
        }
    }
}
