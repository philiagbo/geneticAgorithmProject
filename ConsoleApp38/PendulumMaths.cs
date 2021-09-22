using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACW
{
    class PendulumMaths
    {

        double Kp;
        double Kl;
        double g;
        double length;
        double Vmotor;
        double Vbreak;
        double deltaUV;

        double deltaT;

        double chainMin;
        double chainMax;
        double worldWidth;

        int stepsPerRun;
        int currentStep;

        int crabNum;
        double fitness;


        double[] phi;
        double[] omega;
        double[] v;
        double[] u;
        double[] x;

        public static double[] lengths = new double[6] { 0.35, 0.35, 0.35, 0.35, 0.35, 0.35 };


        public static int num_carts;
        public static double w_of_cart;
        public static double length_of_chain;
        public static double w_of_half_world;
        public static double scale;
        public static double h_of_cart;

        public void initialise(int mycrabnum)
        {
            crabNum = mycrabnum;
            phi = new double[mycrabnum];
            omega = new double[mycrabnum];
            v = new double[mycrabnum];
            u = new double[mycrabnum];
            x = new double[mycrabnum];


            Kp = 0.005;
            Kl = 0.05;
            g = 9.81;
            length = lengths[crabNum];
            Vmotor = 7.0;
            Vbreak = 8.5;
            deltaUV = 0.05;

            deltaT = 0.01;

            chainMin = 0.05;
            chainMax = 0.35;
            worldWidth = 2.0;

            stepsPerRun = 3000;
            currentStep = 0;

            initialPosition();
            length_of_chain = lengths[0];
            w_of_cart = chainMin;
            length_of_chain = chainMax - chainMin;
            w_of_half_world = (worldWidth / 2.0);
            scale = 800 / (2 * w_of_half_world);
            h_of_cart = (w_of_cart / 2);
            num_carts = crabNum;
        }

        public void initialPosition()
        {

            if (crabNum > 0)
                phi[0] = 0.8 * Math.PI;
            if (crabNum > 1)
                phi[1] = 0.9 * Math.PI;
            if (crabNum > 2)
                phi[2] = 1.0 * Math.PI;
            if (crabNum > 3)
                phi[3] = 1.1 * Math.PI;
            if (crabNum > 4)
                phi[4] = 1.2 * Math.PI;

            if (crabNum > 0)
                x[0] = 0.0 + 0.25 * (-4.0) * (chainMin + chainMax);
            if (crabNum > 1)
                x[1] = 0.0 + 0.25 * (-2.0) * (chainMin + chainMax);
            if (crabNum > 2)
                x[2] = 0.0 + 0.25 * 0.0 * (chainMin + chainMax);
            if (crabNum > 3)
                x[3] = 0.0 + 0.25 * 2.0 * (chainMin + chainMax);
            if (crabNum > 4)
                x[4] = 0.0 + 0.25 * 4.0 * (chainMin + chainMax);

            for (int j = 0; j < crabNum; j++)
            {
                omega[j] = 0.0;
                v[j] = 0.0;
                u[j] = 0.0;
            }

            fitness = 0.0;
            currentStep = 0;
        }

        public double motor(double u, double v)
        {
            if (Math.Abs(u - v) > deltaUV)
            {
                if (u > v)
                {
                    if (v >= 0)
                        return Vmotor;
                    else
                        return Vbreak;
                }
                else
                {
                    // u<=v
                    if (v >= 0)
                        return -Vbreak;
                    else
                        return -Vmotor;
                }
            }
            else
            {
                // Math.Abs(u-v)<=deltaUV
                if (u >= v)
                {
                    if (v >= 0)
                        return Vmotor / deltaUV * (u - v);
                    else
                        return Vbreak / deltaUV * (u - v);
                }
                else
                {
                    // u<v
                    if (v >= 0)
                        return Vbreak / deltaUV * (u - v);
                    else
                        return Vmotor / deltaUV * (u - v);
                }
            }
        }

        public double changeAngularVel(double phi, double omega,
                     double u, double v)
        {

            if (omega == 0.0)
                return 3.0 * g / (2.0 * length) * Math.Sin(phi) - 3.0 / (2.0 * length) * motor(u, v) * Math.Cos(phi);
            else
                return 3.0 * g / (2.0 * length) * Math.Sin(phi) - 3.0 / (2.0 * length) * motor(u, v) * Math.Cos(phi)
                - Kp * omega * Math.Abs(omega) - Kl * omega / Math.Abs(omega);

        }

        public int checkForBorders()
        {
            if (currentStep > stepsPerRun)
            {
                return 0;
            }

            for (int j = 0; j < crabNum; j++)
            {
                // no room to the right:
                if (j < crabNum - 1)
                {
                    if ((Math.Abs(x[j] - x[j + 1]) < chainMin)
                    && ((v[j] > 0.0) || (v[j] == 0.0 && u[j] > 0.0)))
                    {
                        //printf("%d knocks over %d to the right. (%f)\n", j, j+1, Math.Abs(x[j]-x[j+1]));
                        v[j] = 0.0;

                        return 0;
                    }
                }
                if (j > 0)
                {
                    if ((Math.Abs(x[j] - x[j - 1]) > chainMax)
                    && ((v[j] > 0.0) || (v[j] == 0.0 && u[j] > 0.0)))
                    {
                        v[j] = 0.0;
                        //printf("%d snaps the chain by going right. (%f)\n", j, Math.Abs(x[j]-x[j-1]));

                        return 0;
                    }
                }
                // no room to the left:
                if (j < crabNum - 1)
                {
                    if ((Math.Abs(x[j] - x[j + 1]) > chainMax)
                    && ((v[j] < 0.0) || (v[j] == 0.0 && u[j] < 0.0)))
                    {
                        v[j] = 0.0;
                        //printf("%d snaps the chain by going left. (%f)\n", j, Math.Abs(x[j]-x[j+1]));

                        return 0;
                    }
                }
                if (j > 0)
                {
                    if ((Math.Abs(x[j] - x[j - 1]) < chainMin)
                    && ((v[j] < 0.0) || (v[j] == 0.0 && u[j] < 0.0)))
                    {
                        v[j] = 0.0;
                        //printf("%d knocks over %d to the left. (%f)\n", j, j-1, Math.Abs(x[j]-x[j-1]));

                        return 0;
                    }
                }


                if (omega[j] > 5.0 * Math.PI)
                {
                    omega[j] = 5.0 * Math.PI;

                    return 0;
                }
                if (omega[j] < -5.0 * Math.PI)
                {
                    omega[j] = -5.0 * Math.PI;

                    return 0;
                }
                if (x[j] < -worldWidth / 2.0)
                {
                    x[j] = -worldWidth / 2.0;
                    v[j] = 0.0;

                    return 0;
                }
                if (x[j] > worldWidth / 2.0)
                {
                    x[j] = worldWidth / 2.0;

                    v[j] = 0.0;
                    return 0;
                }
                if (v[j] < -2.0)
                {
                    v[j] = -2.0;

                    return 0;
                }
                if (v[j] > 2.0)
                {

                    v[j] = 2.0;
                    return 0;
                }
            }
            return 1;
        }

        public double[][] getSensorValues()
        {

            double[][] sensorValues;
            sensorValues = new double[crabNum][];
            double distLeft;
            double distRight;


            for (int i = 0; i < sensorValues.Length; i++)
            {
                sensorValues[i] = new double[10];
            }


            for (int i = 0; i < crabNum; i++)
            {

                distLeft = 1.0;
                distRight = 1.0;

                if (crabNum > 1)
                {
                    if (i == 0)
                        distRight = Math.Abs(x[0] - x[1]);
                    else if (i == crabNum - 1)
                        distLeft = Math.Abs(x[crabNum - 1] - x[crabNum - 2]);
                    else
                    {
                        distLeft = Math.Abs(x[i] - x[i - 1]);
                        distRight = Math.Abs(x[i] - x[i + 1]);
                    }
                }

                while (phi[i] < 0.0)
                    phi[i] += 2.0 * Math.PI;
                while (phi[i] > 2.0 * Math.PI)
                    phi[i] -= 2.0 * Math.PI;

                if (phi[i] < 0.5 * Math.PI)
                {
                    sensorValues[i][0] = 127 - (int)(127.0 * phi[i] / (0.5 * Math.PI));
                    sensorValues[i][1] = 0;
                    sensorValues[i][2] = 0;
                    sensorValues[i][3] = 0;
                }
                else if (phi[i] < Math.PI)
                {
                    sensorValues[i][0] = 0;
                    sensorValues[i][1] = 0;
                    sensorValues[i][2] = 127 - (int)(127.0 * (phi[i] - 0.5 * Math.PI) / (0.5 * Math.PI));
                    sensorValues[i][3] = 0;
                }
                else if (phi[i] < 1.5 * Math.PI)
                {
                    sensorValues[i][0] = 0;
                    sensorValues[i][1] = (int)(-127.0 * (Math.PI - phi[i]) / (0.5 * Math.PI));
                    sensorValues[i][2] = 0;
                    sensorValues[i][3] = 0;
                }
                else
                {
                    sensorValues[i][0] = 0;
                    sensorValues[i][1] = 0;
                    sensorValues[i][2] = 0;
                    sensorValues[i][3] = (int)(-127.0 * (1.5 * Math.PI - phi[i]) / (0.5 * Math.PI));
                }

                if (x[i] < 0.0)
                {
                    if ((worldWidth / 4.0) + x[i] < distLeft)
                        sensorValues[i][4] = (int)(-1.0 * 127.0 * x[i] / (worldWidth / 2.0));
                    else
                        sensorValues[i][4] = (int)(127.0 * distLeft / (worldWidth / 2.0));
                    if (distRight < (worldWidth / 4.0))
                    {
                        sensorValues[i][5] = (int)(127.0 * distRight / (worldWidth / 2.0));
                    }
                    else
                    {
                        sensorValues[i][5] = 0;
                    }
                }
                else
                {
                    if ((worldWidth / 4.0) - x[i] < distRight)
                    {
                        sensorValues[i][5] = (int)(127.0 * x[i] / (worldWidth / 2.0));
                    }
                    else
                    {
                        sensorValues[i][5] = (int)(127.0 * distRight / (worldWidth / 2.0));
                    }
                    if (distLeft < (worldWidth / 4.0))
                        sensorValues[i][4] = (int)(127.0 * distLeft / (worldWidth / 2.0));
                    else
                        sensorValues[i][4] = 0;
                }
                if (v[i] < 0.0)
                {
                    sensorValues[i][6] = (int)(-1.0 * 127.0 * v[i] / 2.0);
                    sensorValues[i][7] = 0;
                }
                else
                {
                    sensorValues[i][6] = 0;
                    sensorValues[i][7] = (int)(127.0 * v[i] / 2.0);
                }
                if (omega[i] < 0.0)
                {
                    sensorValues[i][8] = (int)(-1.0 * 127.0 * omega[i] / (5.0 * Math.PI));
                    sensorValues[i][9] = 0;
                }
                else
                {
                    sensorValues[i][8] = 0;
                    sensorValues[i][9] = (int)(127.0 * omega[i] / (5.0 * Math.PI));
                }


                for (int ii = 0; ii < sensorValues.Length; ii++)
                {
                    for (int xx = 0; xx < 10; xx++)
                    {
                        //noise
                        //sensorValues[ii][xx] += Program._rand.Next() % 6 - 3;

                        if (sensorValues[ii][xx] < 0)
                            sensorValues[ii][xx] = 0;
                    }
                }

            }
            return sensorValues;
        }

        public double getFitness()
        {
            return fitness;
        }

        public int getcrabnum()
        {
            return crabNum;
        }



        public int performOneStep(double[][] motorControl)
        {

            double phiD1, phiD2, phiD3;
            double omegaD1, omegaD2, omegaD3;
            double vD1, vD2, vD3;
            double xD1, xD2, xD3;

            for (int j = 0; j < crabNum; j++)
            {

                double a1 = (double)motorControl[j][0] / 127.0;
                double a2 = (double)motorControl[j][1] / 127.0;

                u[j] = 2.0 * (a1 - a2);

                // runge-kutta:
                phiD1 = omega[j];
                xD1 = v[j];
                omegaD1 = changeAngularVel(phi[j], omega[j], u[j], v[j]);
                vD1 = motor(u[j], v[j]);
                omegaD2 = changeAngularVel(phi[j] + phiD1 * deltaT * 0.5,
                               omega[j] + omegaD1 * deltaT * 0.5,
                               u[j],
                               v[j] + vD1 * deltaT * 0.5);
                phiD2 = omega[j] + omegaD1 * deltaT * 0.5;
                xD2 = v[j] + vD1 * deltaT * 0.5;
                vD2 = motor(u[j], v[j] + vD1 * deltaT * 0.5);
                omegaD3 = changeAngularVel(phi[j] - phiD1 * deltaT + phiD2 * deltaT * 2.0,
                               omega[j] - omegaD1 * deltaT + omegaD2 * deltaT * 2.0,
                               u[j],
                               v[j] + vD1 * deltaT + vD2 * deltaT * 2.0);
                phiD3 = omega[j] - omegaD1 * deltaT + omegaD2 * deltaT * 2.0;
                xD3 = v[j] - vD1 * deltaT + vD2 * deltaT * 2.0;
                vD3 = motor(u[j], v[j] - vD1 * deltaT + vD2 * deltaT * 2.0);

                phi[j] += deltaT * (1.0 / 6.0 * phiD1 + 4.0 / 6.0 * phiD2 + 1.0 / 6.0 * phiD3);
                x[j] += deltaT * (1.0 / 6.0 * xD1 + 4.0 / 6.0 * xD2 + 1.0 / 6.0 * xD3);
                omega[j] += deltaT * (1.0 / 6.0 * omegaD1 + 4.0 / 6.0 * omegaD2 + 1.0 / 6.0 * omegaD3);
                v[j] += deltaT * (1.0 / 6.0 * vD1 + 4.0 / 6.0 * vD2 + 1.0 / 6.0 * vD3);



                fitness += Math.Abs(phi[j] - Math.PI) / (Math.PI * crabNum * (float)stepsPerRun);

            } // for crab num

            currentStep++;
            return checkForBorders();
        }


        public double[][] getState()
        {

            double[][] state = new double[crabNum][];

            for (int i = 0; i < 5; i++)
            {
                state[i] = new double[5];
            }

            for (int i = 0; i < crabNum; i++)
            {
                state[i][0] = u[i];
                state[i][1] = x[i];
                state[i][2] = phi[i];
                state[i][3] = omega[i];
                state[i][4] = v[i];
            }

            return state;
        }
    }
}
