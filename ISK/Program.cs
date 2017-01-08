using GAF;
using GAF.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISK
{
    class Program
    {
        //bool minColorsNo = false;
        //const double crossoverProbability = 0.98;
        //const int elitismPercentage = 3;
        //const double mutationProbability = 0.002;

        //static bool minColorsNo = true;
        //const double crossoverProbability = 0.98;
        //const int elitismPercentage = 3;
        //const double mutationProbability = 0.002;

        static Random rand = new Random();
        static Data data;

        static DateTime startTime;

        static int lastEvoLen;

        private static void Main(string[] args)
        {
            data = new Data("dane/25.txt", true);

            double mutation = 0.002;
            double cross = 0.98;
            double elite = 0.08;
            double bestMutation = mutation;
            double bestCross = cross;
            double bestElite = elite;
            int bestEvoLen = 999999;

            while (true)
            {
                int evoLen = 0;
                for (int i = 0; i < 5; i++)
                    evoLen += Run(cross, mutation, elite);
                if (bestEvoLen > evoLen)
                {
                    bestEvoLen = evoLen;
                    bestMutation = mutation;
                    bestCross = cross;
                    bestElite = elite;
                }

                Console.WriteLine("\r\t\t\t\t\t\r" + mutation + "\t" + cross + "\t" + elite + "\t" + evoLen);

                mutation = RandomDouble(System.Math.Max(0.0, bestMutation - 0.001), System.Math.Min(1.0, bestMutation + 0.001));
                cross = RandomDouble(System.Math.Max(0.0, bestCross - 0.02), System.Math.Min(1.0, bestCross + 0.02));
                elite = RandomDouble(System.Math.Max(0.005, bestElite - 0.01), System.Math.Min(1.0, bestElite + 0.01));
            }
        }

        static public double RandomDouble(double min, double max)
        {
            double range = max - min;
            return rand.NextDouble() * range + min;
        }

        static public int Run(double crossoverProbability, double mutationProbability, double eliteFraction)
        {
            //create a Population
            var population = new Population();
            for (int c = 0; c < 1000; c++)
            {
                var chromosome = new Chromosome();
                for (int g = 0; g < data.EdgesCount; g++)
                    chromosome.Genes.Add(new Gene(rand.Next(data.PosibleGenesNo)));
                population.Solutions.Add(chromosome);
            }

            //create the genetic operators 
            var elite = new Elite(Convert.ToInt32(eliteFraction * 100));

            var crossover = new Crossover(crossoverProbability, true)
            {
                CrossoverType = CrossoverType.SinglePoint
            };

            var mutation = new MutateInt(mutationProbability, data.PosibleGenesNo);

            //create the GA itself 
            var ga = new GeneticAlgorithm(population, EvaluateFitness);

            //subscribe to the GAs Generation Complete event 
            ga.OnGenerationComplete += ga_OnGenerationComplete;
            ga.OnRunComplete += ga_OnRunComplete;

            //add the operators to the ga process pipeline 
            ga.Operators.Add(elite);
            ga.Operators.Add(crossover);
            ga.Operators.Add(mutation);

            //run the GA 
            startTime = DateTime.Now;
            ga.Run(TerminateAlgorithm);

            return lastEvoLen;
        }

        public static double EvaluateFitness(Chromosome chromosome)
        {
            if (chromosome != null)
            {
                int fitness = 0;
                var globalColorsUses = new int[data.PosibleGenesNo];

                foreach (var edgeGroup in data.EdgesOfVertex)
                {
                    var colorsUses = new int[data.PosibleGenesNo];
                    foreach (var edgeNo in edgeGroup)
                    {
                        int color = (int)chromosome.Genes[edgeNo].ObjectValue;
                        colorsUses[color]++;
                        globalColorsUses[color]++;
                    }
                    foreach (var colorUse in colorsUses)
                    {
                        if (colorUse > 1)
                            fitness += colorUse - 1;
                    }
                }

                int colorsUsed = 0;
                foreach (var color in globalColorsUses)
                {
                    if (color > 0)
                        colorsUsed++;
                }
                if (colorsUsed > data.ChromaticIndex)
                    fitness += colorsUsed - data.ChromaticIndex;
                if (colorsUsed < data.ChromaticIndex - 1)
                    fitness += data.ChromaticIndex - colorsUsed - 1;

                return 1.0 / (fitness + 1);
            }
            else
            {
                //chromosome is null
                throw new ArgumentNullException("chromosome", "The specified Chromosome is null.");
            }
        }

        static bool TerminateAlgorithm(Population population, int currentGeneration, long currentEvaluation)
        {
            return currentGeneration > 200 || population.MaximumFitness >= 1.0;
        }

        static void ga_OnGenerationComplete(object sender, GaEventArgs e)
        {
            TimeSpan time = DateTime.Now - startTime;

            //get the best solution 
            var chromosome = e.Population.GetTop(1)[0];

            int fitness = Convert.ToInt32(1.0 / e.Population.MaximumFitness);

            //display the X, Y and fitness of the best chromosome in this generation 
            //Console.WriteLine("{0}\t{1}\t{2}", time, e.Generation, fitness);
            Console.Write("\r\t\t\t\t\t\r{0}\t{1}", e.Generation, fitness);
        }

        static void ga_OnRunComplete(object sender, GaEventArgs e)
        {
            lastEvoLen = e.Generation;
        }
    }
}
