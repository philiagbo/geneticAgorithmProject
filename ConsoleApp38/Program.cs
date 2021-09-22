using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace ACW
{

    class Program
    {
        public static Random _rand = new Random();
        public static List<List<double>> population = new List<List<double>>();
        
        public static List<List<double>> TournamentListValues = new List<List<double>>();
        public static List<double> TournamentScores = new List<double>();
        public static int HighestScorePosition;
        public static List<double> TournamentValuesContainerA = new List<double>();
        public static List<double> TournamentValuesContainerB = new List<double>();
         
        public static List<List<double>> Children = new List<List<double>>();
        public static int Count = 0;
        public static int Generation = 1;

        static void Main(string[] args)
        {
            //stores the results in a text file
            using (StreamWriter BestScore = new StreamWriter("BestScore.txt")) ;
            using (StreamWriter AverageScore = new StreamWriter("AverageScore.txt")) ;


                List<List<double>> mainChildren = new List<List<double>>();
            
            
            //creates a 100 population members with 60 random weights
            for (int i = 0; i < 100; i++)
            {
                //Create a list to store some random doubles 
                List<double> randomWeights = new List<double>();

                //adds 60 random doubles to randomweights list between [0,1]
                for (double j = 0; j < 60; j++)
                {

                    randomWeights.Add((_rand.NextDouble() * 2) - 1);

                }
                population.Add(randomWeights);
            }

            //  writeln the lists
            for (int i = 0; i < 40; i++)
            {
                getScores();
                //creates 100 children for one generation 
                for (int j = 0; j < 100; j++)
                {                   
                    GetTournamentValues();
                    mainChildren = Crossover();
                    Reset();                  
                }

                if (Count == 100)
                {

                    Console.WriteLine("Generation " + Generation);
                    Generation++;
                    population.Clear();
                    for (int k = 0; k < 100; k++)
                    {
                        population.Add(mainChildren[k]);
                    }
                    mainChildren.Clear();
                    Children.Clear();
                    
                    TournamentListValues.Clear();
                    TournamentScores.Clear();

                    
                    Count = 0;


                }
            }
            Console.WriteLine("done");
            Console.ReadLine();

            //Add 60 random doubles to the lists (between the ranges [0,1]
            //These represent random weights 


            


            //Print the result

        }



        public static void getScores()
        {
            List<double> scores = new List<double>();
            double highestScore = 0;
            double T = 0;
            double sum = 0;
            for (int i = 0; i < 100; i++)
            {
                
                T = GetResults(population[i]);
                scores.Add(T);
                

                foreach (double item in scores)
                {
                    double currentScore = 0;
                    highestScore = item;
                    if (currentScore > highestScore)
                    {
                        highestScore = currentScore;
                       
                    }

                    sum += item;
                }
                
            }
            double mean = sum / scores.Count;
            Console.WriteLine("average score: " + mean);
            Console.WriteLine("best score: " + highestScore);


            using (StreamWriter AverageScore = new StreamWriter ("AverageScore.txt", append: true))
            {
                AverageScore.WriteLine(mean);
            }

            using (StreamWriter BestScore = new StreamWriter("BestScore.txt", append: true))
            {
                BestScore.WriteLine(highestScore);
            }
            
        }

        


        public static void GetTournamentValues()
        {
            
            
                
                for (int i = 0; i < 4; i++)
                {
                    int member = _rand.Next(0, 99);
                    TournamentListValues.Add(population[member]);
                    TournamentScores.Add(GetResults(TournamentListValues[i]));
                }
                HighestScorePosition = TournamentScores.IndexOf(TournamentScores.Max());

                
                for (int i = 0; i < 60; i++)
                {
                    TournamentValuesContainerA.Add(TournamentListValues[HighestScorePosition][i]);
                }
                


                TournamentScores[HighestScorePosition] = 0;
                HighestScorePosition = TournamentScores.IndexOf(TournamentScores.Max());


                
                for (int i = 0; i < 60; i++)
                {
                    TournamentValuesContainerB.Add(TournamentListValues[HighestScorePosition][i]);
                }
               
            

        }

        
        // make a random var of crossover between 0 and 1
        public static List<List<double>> Crossover()
        {
            double crossoverRate = _rand.NextDouble();
            List<double> Child = new List<double>();
            if (crossoverRate < 0.5)
            {
                for (int i = 0; i < 30; i++)
                {
                    Child.Add(TournamentValuesContainerA[i]);

                }

                for (int i = 30; i < 60; i++)
                {
                    Child.Add(TournamentValuesContainerB[i]);
                }

                
                

            }
            else 
            {
                for (int i = 0; i < 60; i++)
                {
                    Child.Add(TournamentValuesContainerA[i]);
                }
                //Children.Add(Child);
                
            }

            for (int i = 0; i < Child.Count; i++)
            {
                double mutationRate = _rand.NextDouble();

                if (mutationRate < 0.05)
                {
                    double RandomMutation = (_rand.NextDouble() * 2) - 1;
                    Child[i] = RandomMutation;

                }

            }
            Children.Add(Child);
            Count++;

            return Children;
            //Child.Clear();
            
        }

        

        public static void Reset()
        {
     
            
            

                TournamentScores.Clear();
                TournamentListValues.Clear();
                TournamentValuesContainerA.Clear();
                TournamentValuesContainerB.Clear();
                //Child.Clear();
            
        }

        public static double GetResults(List<double> weights)
        {
            Network net = new Network();

            net.SetWeights(weights);

            PendulumMaths p = new PendulumMaths();
            p.initialise(1);

            Network v = new Network();
            v.SetWeights(net.GetWeights());

            double[][] motor_vals = new double[p.getcrabnum()][];

            for (int i = 0; i < motor_vals.Length; i++)
            {
                motor_vals[i] = new double[2];
            }

            do
            {
                double[][] sval = (p.getSensorValues());

                double[] inputs = new double[10];

                for (int i = 0; i < p.getcrabnum(); i++)
                {

                    for (int x = 0; x < sval[0].Length; x++)
                    {
                        inputs[x] = ((sval[i][x]) / (127) * (1 - 0)) + 1;
                    }

                    v.SetInputs(inputs);

                    v.Execute();

                    double[] outputs = v.GetOutputs();

                    motor_vals[i][0] = ((outputs[0])) * 127;
                    motor_vals[i][1] = ((outputs[1])) * 127;

                }

            }
            while (p.performOneStep(motor_vals) == 1);

            return p.getFitness();
        }

        
    }
    
}