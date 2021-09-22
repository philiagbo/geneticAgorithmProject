using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACW
{

    class Network
    {
        double[][] layers;

        int[] layersSize = new int[] { 10, 5, 2 };

        double[][] connections;

        public Network()
        {
            layers = new double[layersSize.Length][];
            connections = new double[layersSize.Length - 1][];

            for (int i = 0; i < layersSize.Length; i++)
            {
                if (i != layers.Length - 1)
                {
                    connections[i] = new double[layersSize[i] * layersSize[i + 1]];
                }

                layers[i] = new double[layersSize[i]];
            }

            for (int i = 0; i < layers.GetLength(0); i++)
            {
                for (int x = 0; x < layers[i].Length; x++)
                {
                    layers[i][x] = 0.5;
                }
            }

            for (int i = 0; i < connections.GetLength(0); i++)
            {
                for (int x = 0; x < connections[i].Length; x++)
                {
                    connections[i][x] = (Program._rand.NextDouble() * 2) - 1;
                }
            }
        }

        public void SetInputs(double[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                layers[0][i] = inputs[i];
            }
        }

        public void Execute()
        {
            for (int i = 1; i < layers.GetLength(0); i++)
            {
                //This loops over every gene in the layer
                int count = 0;
                for (int x = 0; x < layers[i].Length; x++)
                {
                    double result = 0;

                    for (int y = 0; y < layers[i - 1].Length; y++)
                    {
                        result = result + layers[i - 1][y] * connections[i - 1][count];
                        count++;
                    }

                    layers[i][x] = (1.0 / (1.0 + Math.Exp(-10 * (result))));
                }

                count = 0;
            }

        }

        public double[] GetOutputs()
        {

            double[] outputs = new double[layers[layersSize.Length - 1].Length];

            for (int i = 0; i < layers[layersSize.Length - 1].Length; i++)
            {
                outputs[i] = layers[layersSize.Length - 1][i];
            }

            return outputs;
        }

        public List<double> GetWeights()
        {
            List<double> weights = new List<double>();

            for (int i = 0; i < connections.GetLength(0); i++)
            {
                for (int x = 0; x < connections[i].Length; x++)
                {
                    weights.Add(connections[i][x]);
                }
            }

            return weights;
        }

        public double SetWeights(List<double> weights)
        {
            double s = 0;
            int count = 0;

            for (int i = 0; i < connections.GetLength(0); i++)
            {
                for (int x = 0; x < connections[i].Length; x++)
                {
                    connections[i][x] = weights[count];
                    s = s + connections[i][x];
                    count++;
                }
            }

            return s;
        }
    }
}
