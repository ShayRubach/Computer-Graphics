﻿/*
 * ran shoshan 308281575
 * shay rubach 305687352
 * yaniv yona 203455266
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace ComputerGraphics {
    /* 
     * This module implements all of the required transformations: Rotation, Scaling, Transition
     * Also includes the Matrix utility functions.
     */
    static class Transformations {

        public static readonly Point3D shapesMiddlePt = new Point3D(100.0, -100.0, -50.0);
        public static readonly Point3D userPov = new Point3D(0.0, 0.0, -1000.0);

        //min and max values for Z axis distance (artificial) on screen:
        public static readonly Point zDistanceMinMax = new Point(-1000.0, 1000.0);

        public static Point screen;

        //scale polygon
        public static Point3D Scale(Point3D pt, double scaleValue) {
            return new Point3D(pt.X * scaleValue, pt.Y * scaleValue, pt.Z * scaleValue);
        }

        internal static Point3D Rotate(Point3D vertex, Axis axis, double angle) {

            //calculate offset values
            var xCenter = Math.Abs(screen.X / 2);
            var yCenter = Math.Abs(screen.Y / 2);
            var zCenter = Math.Abs(zDistanceMinMax.X - zDistanceMinMax.Y) / 2;

            //our distance vector - we need this?
            //double[] distanceVector = { xCenter, yCenter, zCenter };

            //base matrix representation of our vertex:
            double[,] baseVertexMtx = { { vertex.X, vertex.Y, vertex.Z, 1 } };

            //post rotation processed matrix
            double[,] rotatedMtx;

            switch (axis) {
                default:
                case Axis.X:
                    rotatedMtx = MtxHelper.MtxMultiply(baseVertexMtx, RotateXMatrix(angle));
                    break;
                case Axis.Y:
                    rotatedMtx = MtxHelper.MtxMultiply(baseVertexMtx, RotateYMatrix(angle));
                    break;
                case Axis.Z:
                    rotatedMtx = MtxHelper.MtxMultiply(baseVertexMtx, RotateZMatrix(angle));
                    break;
            }
            
            //return new rotated 3d point:
            return new Point3D(rotatedMtx[0,0], rotatedMtx[0,1], rotatedMtx[0,2]);
        }

        //matrix builder functions:
        //matrix for X rotation
        private static double[,] RotateXMatrix(double angle) {
            //refactor this sentence and use in other places:
            angle = (angle * (Math.PI / 180));
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            double[,] mtx = {
                { 1, 0, 0, 0 },
                { 0, cos, sin, 0 },
                { 0, -sin, cos, 0 },
                { 0, 0, 0, 1 }
            };

            return mtx;
        }

        //matrix for Y rotation
        private static double[,] RotateYMatrix(double angle) {
            angle = (angle * (Math.PI / 180));
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            double[,] mtx = {
                { cos, 0, -sin, 0 },
                { 0, 1, 0, 0 },
                { sin, 0, cos, 0 },
                { 0, 0, 0, 1 }
            };
            return mtx;
        }

        //matrix for Z rotation
        private static double[,] RotateZMatrix(double angle) {
            angle = (angle * (Math.PI / 180));
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            double[,] mtx = {
                { cos, sin, 0, 0 },
                { -sin, cos, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };
            return mtx;
        }


        internal static Point3D Transition(Point3D vertex, Axis axis, double value) {

            //base matrix representation of our vertex:
            double[,] baseVertexMtx = { { vertex.X, vertex.Y, vertex.Z, 1 } };

            //post transition processed matrix:
            double[,] transitionedMtx = MtxHelper.MtxMultiply(baseVertexMtx, TransitioningMatrix(axis,value));

            //return new rotated 3d point:
            return new Point3D(transitionedMtx[0, 0], transitionedMtx[0, 1], transitionedMtx[0, 2]);
        }

        public static double[,] TransitioningMatrix(Axis axis, double value) {

            Point3D pt = new Point3D(1.0,1.0,1.0);
            switch (axis) {
                case Axis.X: pt.X = value; break;
                case Axis.Y: pt.Y = value; break;
                case Axis.Z: pt.Z = value; break;
            }

            //the transition matrix
            double[,] transitionMtx = {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { pt.X, pt.Y, pt.Z, 1 }
            };
            return transitionMtx;
        }




        //projections implementation:
        //orthographic:
        public static Point3D Orthographic(Point3D vertex) {

            //base matrix representation of our vertex:
            double[,] baseVertexMtx = { { vertex.X, vertex.Y, vertex.Z, 1 } };

            //orthographic matrix representation
            double[,] orthographicMtx = { 
                { 1, 0, 0, 0 }, 
                { 0, 1, 0, 0 }, 
                { 0, 0, 1, 0 }, 
                { 0, 0, 0, 1 }
            };

            //resulted processed matrix
            double[,] processedMtx = MtxHelper.MtxMultiply(baseVertexMtx, orthographicMtx);

            return new Point3D(processedMtx[0, 0], processedMtx[0, 1], processedMtx[0, 2]);
        }


        //oblique:
        public static Point3D Oblique(Point3D vertex, double angle = 90.0) {
            //base matrix representation of our vertex:
            double[,] baseVertexMtx = { { vertex.X, vertex.Y, vertex.Z, 1 } };

            //cos and sin angle calculation
            var cos = (Math.Cos(angle * (Math.PI / 180)));
            var sin = (Math.Sin(angle * (Math.PI / 180)));

            //oblique matrix representation (Cabinet)
            double[,] obliqueMtx = { 
                { 1, 0, 0, 0 }, 
                { 0, 1, 0, 0 }, 
                { cos/2, sin/2, 1, 0 }, 
                { 0, 0, 0, 1 }
            };
            
            //resulted processed matrix
            double[,] processedMtx = MtxHelper.MtxMultiply(baseVertexMtx, obliqueMtx);
            return new Point3D(processedMtx[0, 0], processedMtx[0, 1], processedMtx[0,2]);
        }



        //perspective:
        public static Point3D Perspective(Point3D vertex) {
            //base matrix representation of our vertex:
            double[,] baseVertexMtx = { { vertex.X, vertex.Y, vertex.Z, 1 } };

            //calculate S(Z) as in formula
            var sZ = 1 / (1 + (vertex.Z / userPov.Z));

            //perspective matrix representation
            double[,] perspectiveMtx = { 
                { sZ, 0, 0, 0 }, 
                { 0, sZ, 0, 0 }, 
                { 0, 0, 0, 0 }, 
                { 0, 0, 0, 1 }
            };

            //resulted processed matrix
            double[,] processedMtx = MtxHelper.MtxMultiply(baseVertexMtx, perspectiveMtx);
            return new Point3D(processedMtx[0, 0], processedMtx[0, 1], processedMtx[0, 2]);
        }

    }

    //used for matrix to matrix calculation operations, such as multiply:
    internal static class MtxHelper {

        public static double[,] MtxMultiply(double[,] A, double[,] B) {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);
            double temp = 0;
            double[,] kHasil = new double[rA, cB];
            if (cA != rB) {
                Console.WriteLine("matrix can't be multiplied!");
                return null;
            }
            else {
                for (int i = 0; i < rA; i++) {
                    for (int j = 0; j < cB; j++) {
                        temp = 0;
                        for (int k = 0; k < cA; k++) {
                            temp += A[i, k] * B[k, j];
                        }
                        kHasil[i, j] = temp;
                    }
                }
                return kHasil;
            }
        }
    }
}
