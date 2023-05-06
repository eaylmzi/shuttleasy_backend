namespace ShuttleRoute
{
    public class Graph
    {
        // helper function to calculate minimum index to visit.
        private static int FindMinIndex(double[] distances, bool[] visited, int numNodes)
        {
            double minDistance = double.MaxValue;
            int minIndex = -1;

            for (int i = 0; i < numNodes; i++)
            {
                if (!visited[i] && distances[i] < minDistance)
                {
                    minDistance = distances[i];
                    minIndex = i;
                }
            }
            return minIndex;
        }

        public int[] DijkstraAlgorithm(double[][] adjacencyMatrix, int startNode = 0)
        {
            int numNodes = adjacencyMatrix.GetLength(0);
            double[] shortestDistances = new double[numNodes];
            bool[] visited = new bool[numNodes];
            int[] visitedNodes = new int[numNodes];
            // initialize arrays
            for (int i = 0; i < numNodes; i++)
            {
                shortestDistances[i] = double.MaxValue;
                visited[i] = false;
                visitedNodes[i] = -1;
            }

            shortestDistances[startNode] = 0; // shortest distance to starting node is 0

            // iterate through all nodes
            for (int i = 0; i < numNodes - 1; i++)
            {
                int minIndex = FindMinIndex(shortestDistances, visited, numNodes);
                visited[minIndex] = true; // mark node as visited
                visitedNodes[i] = minIndex;
                // update distances of adjacent nodes
                for (int j = 0; j < numNodes; j++)
                {
                    if (!visited[j] && adjacencyMatrix[minIndex][j] != double.MaxValue)
                    {
                        double distance = shortestDistances[minIndex] + adjacencyMatrix[minIndex][j];
                        if (distance < shortestDistances[j])
                        {
                            shortestDistances[j] = distance;
                        }
                    }
                }
            }
            visitedNodes[numNodes - 1] = FindMinIndex(shortestDistances, visited, numNodes);
            return visitedNodes;
        }
    }
}
