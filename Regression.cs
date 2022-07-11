using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Statistics.Models.Regression.Linear;
using Accord.Math.Optimization.Losses;

public class Regression : MonoBehaviour
{
    double a1, b1, c1, d1, a2, b2, c2, d2;
    double a3, b3, c3, d3, a4, b4, c4, d4;

    public void Train(string input)
    {
        var data = input.Split('|');
        var hand = data[1];
        double[][] inputs = {
            new double[] { float.Parse(data[2].Split('=')[1]), float.Parse(data[3].Split('=')[1])},
            new double[] { float.Parse(data[4].Split('=')[1]), float.Parse(data[5].Split('=')[1])},
            new double[] { float.Parse(data[6].Split('=')[1]), float.Parse(data[7].Split('=')[1])},
            new double[] { float.Parse(data[8].Split('=')[1]), float.Parse(data[9].Split('=')[1])},
            new double[] { float.Parse(data[10].Split('=')[1]), float.Parse(data[11].Split('=')[1])},
            new double[] { float.Parse(data[12].Split('=')[1]), float.Parse(data[13].Split('=')[1])},
            new double[] { float.Parse(data[14].Split('=')[1]), float.Parse(data[15].Split('=')[1])},
            new double[] { float.Parse(data[16].Split('=')[1]), float.Parse(data[17].Split('=')[1])},
            new double[] { float.Parse(data[18].Split('=')[1]), float.Parse(data[19].Split('=')[1])},
            new double[] { float.Parse(data[20].Split('=')[1]), float.Parse(data[21].Split('=')[1])},
        };

        double[][] outputs = {
            new double[]  { 90,  0},
            new double[]  { 45,  0},
            new double[]  { 0,   0},
            new double[]  { -45, 0},
            new double[]  { -90, 0},
            new double[]  { 0,   90},
            new double[]  { 0,   45},
            new double[]  { 0,   0},
            new double[]  { 0, - 45},
            new double[] { 0, - 90},
        };

        OrdinaryLeastSquares ols = new OrdinaryLeastSquares();
        MultivariateLinearRegression regression = ols.Learn(inputs, outputs);
        double[][] predictions = regression.Transform(inputs);
        double error = new SquareLoss(outputs).Loss(predictions);
        double[] r2 = regression.CoefficientOfDetermination(inputs, outputs);
        print("R2 score: "+ r2[0]);
        print("R2 score: " + r2[1]);

        if (hand.Equals("right"))
        {
            a1 = regression.Weights[0][0];
            b1 = regression.Weights[1][0];
            //c1 = regression.Weights[2][0];
            d1 = regression.Intercepts[0];

            a2 = regression.Weights[0][1];
            b2 = regression.Weights[1][1];
            //c2 = regression.Weights[2][1];
            d2 = regression.Intercepts[1];
        }
        else
        {
            a3 = regression.Weights[0][0];
            b3 = regression.Weights[1][0];
            //c3 = regression.Weights[2][0];
            d3 = regression.Intercepts[0];

            a4 = regression.Weights[0][1];
            b4 = regression.Weights[1][1];
            //c4 = regression.Weights[2][1];
            d4 = regression.Intercepts[1];
        }
        
    }

    public double getAngleX(string hand, double gravY, double yaw)
    {
        if (hand.Equals("right"))
        {
            return a1 * gravY + b1 * yaw +  d1;
        }
        else
        {
            return a3 * gravY + b3 * yaw + d3;
        }
       
    }

    public double getAngleY(string hand, double gravY, double yaw)
    {
        if (hand.Equals("right"))
        {
            return a2 * gravY + b2 * yaw + d2;
        }
        else
        {
            return a4 * gravY + b4 * yaw + d4;
        }
        
    }
    
}