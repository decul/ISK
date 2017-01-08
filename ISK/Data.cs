using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISK
{
    class Data
    {
        public int EdgesCount { get; private set; }
        public int ChromaticIndex { get; private set; }
        public int PosibleGenesNo { get; private set; }

        //public int[,] VerticesOfEdge { get; private set; }
        public int[][] EdgesOfVertex { get; private set; }


        public Data(string filePath, bool minColorsNo = false)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read Edges count
                EdgesCount = int.Parse(reader.ReadLine());

                var eovList = new List<List<int>>(2 * EdgesCount);
                //VerticesOfEdge = new int[EdgesCount, 2];

                // Read data
                for (int e = 0; e < EdgesCount; e++)
                {
                    string[] line = reader.ReadLine().Split(' ');
                    int edgeNo = int.Parse(line[0]) - 1;
                    int v1 = int.Parse(line[1]) - 1;
                    int v2 = int.Parse(line[2]) - 1;

                    //VerticesOfEdge[edgeNo, 0] = v1;
                    //VerticesOfEdge[edgeNo, 1] = v2;

                    // Extend list if new vertices doesn't fit
                    for (int v = eovList.Count; v <= Math.Max(v1, v2); v++)
                        eovList.Add(new List<int>(EdgesCount));

                    // Add edges to vertices (if it's loop, add only once)
                    eovList[v1].Add(edgeNo);
                    if (v1 != v2)
                        eovList[v2].Add(edgeNo);
                }

                // Convert list to array
                EdgesOfVertex = new int[eovList.Count][];
                for (int v = 0; v < eovList.Count; v++)
                    EdgesOfVertex[v] = eovList[v].ToArray();

                // Find chrimatic index and posible genes
                ChromaticIndex = eovList.Max(v => v.Count) + 1;
                if (minColorsNo)
                    PosibleGenesNo = ChromaticIndex;
                else
                    PosibleGenesNo = EdgesCount;
            }
        }
    }
}
