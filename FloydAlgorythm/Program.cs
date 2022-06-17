using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FloydAlgorythm
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateDataFile(2000);
            string path = @"C:\Users\lenovo\OneDrive\Рабочий стол\graph3500.txt";

            int[,] weights = ReadFile(path);
            int[,] directions = FindDirections(weights);

            //Stopwatch timer = new Stopwatch();
            //timer.Start();
            //int[,] researchedArray = Floyd(weights, directions);
            //timer.Stop();
            //TimeSpan timeSpan = timer.Elapsed;

            int tasksCount = 1000;
          
            Stopwatch timer2 = new Stopwatch();
            timer2.Start();
            int[,] researchedArrayParallel = ParallelFloyd(weights, directions, tasksCount);
            timer2.Stop();
            TimeSpan timeSpan2 = timer2.Elapsed;

            //int source = 10;
            //int destination = 1;
            //List<int> shortestPath = FindPath(directions, source, destination);
            //PrintPath(shortestPath);
            //Console.WriteLine($"The total distance from point '{source}' to point '{destination}' is: {researchedArrayParallel[source, destination]}");

            //Console.WriteLine("The time of calculating of all of the shortest distances by sequential method is: " + timeSpan.TotalMilliseconds);
            Console.WriteLine("The time of calculating of all of the shortest distances by parallel  method is: " + timeSpan2.TotalMilliseconds);
            Console.ReadLine();
        }

        public static int[,] ReadFile(string path)
        {
            StreamReader sourceFile = new StreamReader(path);

            int length = FindMaxValue(path) + 1;
            int[,] weights = new int[length, length];


            using(sourceFile)
            {
                string line;
                while((line = sourceFile.ReadLine()) != null)
                {
                    if (line == "")
                        continue;
                    string[] lineArray = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    weights[int.Parse(lineArray[0]), int.Parse(lineArray[1])] = int.Parse(lineArray[2]);
                }
            }
            for(int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (weights[i, j] == 0 && i != j)
                    {
                        weights[i, j] = int.MaxValue;
                    } 
                }
            }
            return weights;
        }

        public static int[,] FindDirections(int[,] weights)
        {
            int length = weights.GetLength(0);
            int[,] distances = new int[length, length];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (weights[i, j] == int.MaxValue)
                    {
                        distances[i, j] = int.MaxValue;
                    }
                    else
                        distances[i, j] = j;
                }
            }
            return distances;
        }

        public static int FindMaxValue(string path)
        {
            StreamReader sourceFile = new StreamReader(path);
            string line;
            int maxValue = 0;
            while ((line = sourceFile.ReadLine()) != null)
            {
                if (line == "")
                    continue;
                string[] lineArray = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (int.Parse(lineArray[0]) > maxValue)
                    maxValue = int.Parse(lineArray[0]);
                if (int.Parse(lineArray[1]) > maxValue)
                    maxValue = int.Parse(lineArray[1]);
            }
            return maxValue;
        }

        public static int[,] Floyd(int[,] weights, int [,] directions)
        {
            int length = weights.GetLength(0);
            for (int k = 0; k < length; k++)
            {
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        if(i != j)
                        {
                            if (weights[i, k] == int.MaxValue || weights[k, j] == int.MaxValue)
                                continue;

                            if (weights[i, k] + weights[k, j] < weights[i, j])
                            {
                                weights[i, j] = weights[i, k] + weights[k, j];
                                directions[i, j] = directions[i, k];
                            }
                        }
                        
                    }
                }
            }
            return weights;
        }

        public static void AlgorythmLoops(int k, int [,] weights, int [,] directions)
        {
            int length = weights.GetLength(0);
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (i != j)
                    {
                        if (weights[i, k] == int.MaxValue || weights[k, j] == int.MaxValue)
                            continue;

                        if (weights[i, k] + weights[k, j] < weights[i, j])
                        {
                            weights[i, j] = weights[i, k] + weights[k, j];
                            directions[i, j] = directions[i, k];
                        }
                    }

                }
            }
        }

        public static int[,] ParallelFloyd(int[,] weights, int[,] directions, int taskCount)
        {
            int nodesInTask = weights.GetLength(0) / taskCount;
            int rest = weights.GetLength(0) % nodesInTask;
            Task[] tasks = new Task[taskCount];
            int index = 0;
            for (int i = 0; i < taskCount; i++)
            {
                if (i == taskCount - 1 && rest > 0)
                {
                    nodesInTask += rest;
                }
                int counter = index;
                Task task = Task.Run(() =>
                {
                    for (int k = counter; k < counter + nodesInTask; k++)
                    {
                        AlgorythmLoops(k, weights, directions);
                    }
                });
                tasks[i] = task;
                index += nodesInTask;
            }
            Task.WaitAll(tasks);
            return weights;
        }

        public static List<int> FindPath(int[,] directions, int source, int destination)
        {
            
            if (directions[source, destination] == int.MaxValue)
                return null;

            List<int> path = new List<int>() { source };
            
            while (destination != source)
            {
                source = directions[source, destination];
                path.Add(source);
            }
            return path;
        }

        public static void PrintPath(List<int> path)
        {
            if (path == null)
            {
                Console.WriteLine("There is no possible path.");
                throw new Exception();
            }
            
            for (int i = 0; i < path.Count; i++)
            {
                string node = $"{path[i]} --> ";
                if(i == path.Count - 1)
                    node = $"{path[i]};\n";

                Console.Write(node);
            }
        }

        public static void CreateDataFile(int length)
        {
            FileStream stream = new FileStream(@"C:\Users\lenovo\OneDrive\Рабочий стол\graph" + length + ".txt", FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(stream);

            using (writer)
            {
                Random rand = new Random();
                for (int i = 0; i < length; i++)
                {
                    int second = rand.Next(0, length);
                    int distance = rand.Next(0, 100);
                    if (i == second)
                    {
                        if (second != length - 1)
                            second++;
                        else
                            second--;
                    }
                    writer.WriteLine($"{i},{second},{distance}");
                }

                for (int i = 0; i < length; i++)
                {

                    int first = rand.Next(0, length);
                    int second = rand.Next(0, length);
                    if (first == second)
                    {
                        if (second != length - 1)
                            second++;
                        else
                            second--;
                    }

                    int distance = rand.Next(0, 100);
                    writer.WriteLine($"{first},{second},{distance}");
                }
            }
        }
    }
}
