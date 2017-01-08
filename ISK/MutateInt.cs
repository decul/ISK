using GAF;
using GAF.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISK
{
    public class MutateInt : MutateBase
    {
        private int possibleGenesNo;
        private Random rand = new Random();


        public MutateInt(double mutationProbability, int possibleGenesNo)
               : base (mutationProbability)
        {
            this.possibleGenesNo = possibleGenesNo;
        }

        protected override void Mutate(Chromosome child)
        {
            base.Mutate(child);
        }

        protected override void MutateGene(Gene gene)
        {
            gene.ObjectValue = (object)rand.Next(possibleGenesNo);
        }
    }
}
