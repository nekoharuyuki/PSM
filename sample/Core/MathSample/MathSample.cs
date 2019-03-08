/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Threading;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;

namespace Sample
{

/**
 * MathSample
 */
class MathSample
{
    private static GraphicsContext graphics;

    static bool loop = true;

    public static void Main(string[] args)
    {
        Init();

        while (loop) {
            SystemEvents.CheckEvents();
            Update();
            Render();
        }

        Term();
    }

    public static bool Init()
    {
        graphics = new GraphicsContext();
        SampleDraw.Init(graphics);

        calcMatrix4();
        calcVector3();
        calcQuaternion();

        return true;
    }

    /// Terminate
    public static void Term()
    {
        SampleDraw.Term();
        graphics.Dispose();
    }

    public static bool Update()
    {
        SampleDraw.Update();

        return true;
    }

    public static bool Render()
    {
        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        SampleDraw.DrawText("Math Sample", 0xffffffff, 0, 0);

        graphics.SwapBuffers();

        return true;
    }

    /// Matrix4 operations examples
    private static void calcMatrix4()
    {
        Matrix4 m1;
        Matrix4 m2;
        Vector3 v1;

        // Matrix4( float ... )
        Console.Write("\nMatrix4( float ... )\n");
        Console.Write("new Matrix4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16) = \n");
        Console.WriteLine(new Matrix4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16));


        // Matrix4( Vector4 ... )
        Console.Write("\nMatrix4( Vector4 ... )\n");
        Console.Write("new Matrix4(new Vector4(1, 2, 3, 4),\n");
        Console.Write("            new Vector4(5, 6, 7, 8),\n");
        Console.Write("            new Vector4(9, 10, 11, 12),\n");
        Console.Write("            new Vector4(13, 14, 15, 16)) = \n");
        Console.WriteLine(new Matrix4(new Vector4(1, 2, 3, 4),
                                    new Vector4(5, 6, 7, 8),
                                    new Vector4(9, 10, 11, 12),
                                    new Vector4(13, 14, 15, 16)));


        // Identity
        Console.Write("\nIdentity\n");
        Console.Write("Matrix4.Identity = \n");
        Console.WriteLine(Matrix4.Identity);


        // operator*( Matrix4, Matrix4 )
        Console.Write("\noperator*( Matrix4, Matrix4 )\n");
        m1 = new Matrix4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        m2 = new Matrix4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);

        Console.Write("Matrix4 m1 = \n");
        Console.WriteLine(m1);

        Console.Write("Matrix4 m2 = \n");
        Console.WriteLine(m2);

        Console.Write("Matrix4 m1 * m2 = \n");
        Console.WriteLine(m1 * m2);


        // Translation
        Console.Write("\nTranslation\n");
        Console.Write("Matrix4.Translation(1, 2, 3) = \n");
        Console.WriteLine(Matrix4.Translation(1, 2, 3));


        // RotationX
        Console.Write("\nRotationX\n");
        Console.Write("Matrix4.RotationX(1) = \n");
        Console.WriteLine(Matrix4.RotationX(1));


        // RotationY
        Console.Write("\nRotationY\n");
        Console.Write("Matrix4.RotationY(1) = \n");
        Console.WriteLine(Matrix4.RotationY(1));


        // RotationZ
        Console.Write("\nRotationZ\n");
        Console.Write("Matrix4.RotationZ(1) = \n");
        Console.WriteLine(Matrix4.RotationZ(1));


        // RotationYxz
        Console.Write("\nRotationYxz\n");
        Console.Write("Matrix4.RotationYxz(1, 2, 3) = \n");
        Console.WriteLine(Matrix4.RotationYxz(1, 2, 3));


        // Scale
        Console.Write("\nScale\n");
        Console.Write("Matrix4.Scale(1, 2, 3) = \n");
        Console.WriteLine(Matrix4.Scale(1, 2, 3));


        // Perspective
        Console.Write("\nPerspective\n");
        Console.Write("Matrix4.Perspective(1, 2, 3, 4) = \n");
        Console.WriteLine(Matrix4.Perspective(1, 2, 3, 4));


        // Frustum
        Console.Write("\nFrustum\n");
        Console.Write("Matrix4.Frustum(1, 2, 3, 4, 5, 6) = \n");
        Console.WriteLine(Matrix4.Frustum(1, 2, 3, 4, 5, 6));


        // Ortho
        Console.Write("\nOrtho\n");
        Console.Write("Matrix4.Ortho(1, 2, 3, 4, 5, 6) = \n");
        Console.WriteLine(Matrix4.Ortho(1, 2, 3, 4, 5, 6));


        // LookAt
        Console.Write("\nLookAt\n");
        Console.Write("Matrix4.LookAt(new Vector3(1, 2, 3),\n");
        Console.Write("               new Vector3(4, 5, 6),\n");
        Console.Write("               new Vector3(7, 8, 9)) = \n");
        Console.WriteLine(Matrix4.LookAt(new Vector3(1, 2, 3),
                                       new Vector3(4, 5, 6),
                                       new Vector3(7, 8, 9)));


        // Inverse
        Console.Write("\nInverse\n");
        m1 = Matrix4.LookAt(new Vector3(1, 2, 3),
                            new Vector3(4, 5, 6),
                            new Vector3(7, 8, 9));
        Console.Write("Matrix4 m1 = \n");
        Console.WriteLine(m1);

        Console.Write("m1.Inverse() = \n");
        Console.WriteLine(m1.Inverse());


        // Transpose
        Console.Write("\nTranspose\n");
        m1 = Matrix4.LookAt(new Vector3(1, 2, 3),
                            new Vector3(4, 5, 6),
                            new Vector3(7, 8, 9));
        Console.Write("Matrix4 m1 = \n");
        Console.WriteLine(m1);

        Console.Write("m1.Transpose() = \n");
        Console.WriteLine(m1.Transpose());


        // TransformPoint
        Console.Write("\nTransformPoint\n");
        m1 = new Matrix4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        v1 = new Vector3(1, 2, 3);

        Console.Write("Matrix4 m1 = \n");
        Console.WriteLine(m1);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("m1.TransformPoint(v1) = \n");
        Console.WriteLine(m1.TransformPoint(v1));


        // TransformVector
        Console.Write("\nTransformVector\n");
        m1 = new Matrix4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        v1 = new Vector3(1, 2, 3);

        Console.Write("Matrix4 m1 = \n");
        Console.WriteLine(m1);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("m1.TransformVector(v1) = \n");
        Console.WriteLine(m1.TransformVector(v1));

        // AxisX AxisY ...
        Console.Write("\nAxisX AxisY ...\n");
        m1 = new Matrix4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);

        Console.Write("Matrix4 m1 = \n");
        Console.WriteLine(m1);

        Console.Write("m1.AxisX = \n");
        Console.WriteLine(m1.AxisX);

        Console.Write("m1.AxisY = \n");
        Console.WriteLine(m1.AxisY);

        Console.Write("m1.AxisZ = \n");
        Console.WriteLine(m1.AxisZ);

        Console.Write("m1.AxisW = \n");
        Console.WriteLine(m1.AxisW);
    }

    /// Vector3 operations examples
    private static void calcVector3()
    {
        Vector3 v1;
        Vector3 v2;

        // Vector3( float ... )
        Console.Write("\nVector3( float ... )\n");
        Console.Write("new Vector3(1, 2, 3) = \n");
        Console.WriteLine(new Vector3(1, 2, 3));


        // Zero One ...
        Console.Write("\nZero One ...\n");

        Console.Write("Vector3.Zero = \n");
        Console.WriteLine(Vector3.Zero);

        Console.Write("Vector3.One = \n");
        Console.WriteLine(Vector3.One);

        Console.Write("Vector3.UnitX = \n");
        Console.WriteLine(Vector3.UnitX);

        Console.Write("Vector3.UnitY = \n");
        Console.WriteLine(Vector3.UnitY);

        Console.Write("Vector3.UnitZ = \n");
        Console.WriteLine(Vector3.UnitZ);


        // opertor+( Vector3, Vector3 )
        Console.Write("\noperator+( Vector3, Vector3 )\n");
        v1 = new Vector3(1, 2, 3);
        v2 = new Vector3(4, 5, 6);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("Vector3 v2 = \n");
        Console.WriteLine(v2);

        Console.Write("Vector3 v1 + v2 = \n");
        Console.WriteLine(v1 + v2);


        // opertor-( Vector3, Vector3 )
        Console.Write("\noperator-( Vector3, Vector3 )\n");
        v1 = new Vector3(1, 2, 3);
        v2 = new Vector3(4, 5, 6);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("Vector3 v2 = \n");
        Console.WriteLine(v2);

        Console.Write("Vector3 v1 - v2 = \n");
        Console.WriteLine(v1 - v2);


        // opertor*( Vector3, Vector3 )
        Console.Write("\noperator*( Vector3, Vector3 )\n");
        v1 = new Vector3(1, 2, 3);
        v2 = new Vector3(4, 5, 6);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("Vector3 v2 = \n");
        Console.WriteLine(v2);

        Console.Write("Vector3 v1 * v2 = \n");
        Console.WriteLine(v1 * v2);


        // opertor/( Vector3, Vector3 )
        Console.Write("\noperator/( Vector3, Vector3 )\n");
        v1 = new Vector3(1, 2, 3);
        v2 = new Vector3(4, 5, 6);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("Vector3 v2 = \n");
        Console.WriteLine(v2);

        Console.Write("Vector3 v1 / v2 = \n");
        Console.WriteLine(v1 / v2);


        // Dot
        Console.Write("\nDot\n");
        v1 = new Vector3(1, 2, 3);
        v2 = new Vector3(4, 5, 6);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("Vector3 v2 = \n");
        Console.WriteLine(v2);

        Console.Write("v1.Dot(v2) = " + v1.Dot(v2) + "\n\n");


        // Cross
        Console.Write("\nCross\n");
        v1 = new Vector3(1, 2, 3);
        v2 = new Vector3(4, 5, 6);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("Vector3 v2 = \n");
        Console.WriteLine(v2);

        Console.Write("v1.Cross(v2) = \n");
        Console.WriteLine(v1.Cross(v2));


        // Length
        Console.Write("\nLength\n");
        v1 = new Vector3(1, 2, 3);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("v1.Length() = " + v1.Length() + "\n\n");


        // Normalize
        Console.Write("\nNormalize\n");
        v1 = new Vector3(1, 2, 3);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("v1.Normalize() = \n");
        Console.WriteLine(v1.Normalize());


        // Distance
        Console.Write("\nDistance\n");
        v1 = new Vector3(1, 2, 3);
        v2 = new Vector3(4, 5, 6);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("Vector3 v2 = \n");
        Console.WriteLine(v2);

        Console.Write("v1.Distance(v2) = " + v1.Distance(v2) + "\n\n");


        // Angle
        Console.Write("\nAngle\n");
        v1 = new Vector3(1, 2, 3);
        v2 = new Vector3(4, 5, 6);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("Vector3 v2 = \n");
        Console.WriteLine(v2);

        Console.Write("v1.Angle(v2) = " + v1.Angle(v2) + "\n\n");


        // Clamp
        Console.Write("\nClamp\n");
        v1 = new Vector3(0.3f, 1.5f, 2.7f);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("v1.Clamp(1, 2) = \n");
        Console.WriteLine(v1.Clamp(1, 2));


        // Repeat
        Console.Write("\nRepeat\n");
        v1 = new Vector3(0.3f, 1.5f, 2.7f);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("v1.Repeat(1, 2) = \n");
        Console.WriteLine(v1.Repeat(1, 2));


        // Lerp
        Console.Write("\nLerp\n");
        v1 = new Vector3(1, 2, 3);
        v2 = new Vector3(4, 5, 6);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("Vector3 v2 = \n");
        Console.WriteLine(v2);

        Console.Write("v1.Lerp(v2, 0.5f) = \n");
        Console.WriteLine(v1.Lerp(v2, 0.5f));


        // Slerp
        Console.Write("\nSlerp\n");
        v1 = new Vector3(1, 2, 3);
        v2 = new Vector3(4, 5, 6);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("Vector3 v2 = \n");
        Console.WriteLine(v2);

        Console.Write("v1.Slerp(v2, 0.5f) = \n");
        Console.WriteLine(v1.Slerp(v2, 0.5f));


        // Swizzle
        Console.Write("\nSwizzle\n");
        v1 = new Vector3(1, 2, 3);

        Console.Write("Vector3 v1 = \n");
        Console.WriteLine(v1);

        Console.Write("v1.Xyz = \n");
        Console.WriteLine(v1.Xyz);

        Console.Write("v1.Zyx = \n");
        Console.WriteLine(v1.Zyx);

        Console.Write("v1.Xy = \n");
        Console.WriteLine(v1.Xy);

        Console.Write("v1.Zy = \n");
        Console.WriteLine(v1.Zy);

        Console.Write("v1.Xyz0 = \n");
        Console.WriteLine(v1.Xyz0);

        Console.Write("v1.Xyz1 = \n");
        Console.WriteLine(v1.Xyz1);
    }

    /// Quaternion operations examples
    private static void calcQuaternion()
    {
        Quaternion q1;
        Quaternion q2;
        Matrix4 m1;

        // Quaternion( float ... )
        Console.Write("\nQuaternion( float ... )\n");
        Console.Write("new Quaternion(1, 2, 3, 4) = \n");
        Console.WriteLine(new Quaternion(1, 2, 3, 4));


        // Identity
        Console.Write("\nIdentity\n");
        Console.Write("Quaternion.Identity = \n");
        Console.WriteLine(Quaternion.Identity);


        // opertor*( Quaternion, Quaternion )
        Console.Write("\noperator*( Quaternion, Quaternion )\n");
        q1 = new Quaternion(1, 2, 3, 4);
        q2 = new Quaternion(5, 6, 7, 8);

        Console.Write("Quaternion q1 = \n");
        Console.WriteLine(q1);

        Console.Write("Quaternion q2 = \n");
        Console.WriteLine(q2);

        Console.Write("Quaternion q1 * q2 = \n");
        Console.WriteLine(q1 * q2);


        // Dot
        Console.Write("\nDot\n");
        q1 = new Quaternion(1, 2, 3, 4);
        q2 = new Quaternion(5, 6, 7, 8);

        Console.Write("Quaternion q1 = \n");
        Console.WriteLine(q1);

        Console.Write("Quaternion q2 = \n");
        Console.WriteLine(q2);

        Console.Write("q1.Dot(q2) = " + q1.Dot(q2) + "\n\n");


        // Length
        Console.Write("\nLength\n");
        q1 = new Quaternion(1, 2, 3, 4);

        Console.Write("Quaternion q1 = \n");
        Console.WriteLine(q1);

        Console.Write("q1.Length() = " + q1.Length() + "\n\n");


        // Normalize
        Console.Write("\nNormalize\n");
        q1 = new Quaternion(1, 2, 3, 4);

        Console.Write("Quaternion q1 = \n");
        Console.WriteLine(q1);

        Console.Write("q1.Normalize() = \n");
        Console.WriteLine(q1.Normalize());


        // Inverse
        Console.Write("\nInverse\n");
        q1 = Quaternion.RotationX(1);

        Console.Write("Quaternion q1 = \n");
        Console.WriteLine(q1);

        Console.Write("q1.Inverse() = \n");
        Console.WriteLine(q1.Inverse());


        // Conjugate
        Console.Write("\nConjugate\n");
        q1 = new Quaternion(1, 2, 3, 4);

        Console.Write("Quaternion q1 = \n");
        Console.WriteLine(q1);

        Console.Write("q1.Conjugate() = \n");
        Console.WriteLine(q1.Conjugate());


        // Angle
        Console.Write("\nAngle\n");
        q1 = Quaternion.RotationX(0);
        q2 = Quaternion.RotationX(1);

        Console.Write("Quaternion q1 = \n");
        Console.WriteLine(q1);

        Console.Write("Quaternion q2 = \n");
        Console.WriteLine(q2);

        Console.Write("q1.Angle(q2) = " + q1.Angle(q2) + "\n\n");


        // Slerp
        Console.Write("\nSlerp\n");
        q1 = Quaternion.RotationX(0);
        q2 = Quaternion.RotationX(1);

        Console.Write("Quaternion q1 = \n");
        Console.WriteLine(q1);

        Console.Write("Quaternion q2 = \n");
        Console.WriteLine(q2);

        Console.Write("q1.Slerp(q2, 0.5f) = \n");
        Console.WriteLine(q1.Slerp(q2, 0.5f));


        // RotationX
        Console.Write("\nRotationX\n");
        Console.Write("Quaternion.RotationX(1) = \n");
        Console.WriteLine(Quaternion.RotationX(1));


        // RotationY
        Console.Write("\nRotationY\n");
        Console.Write("Quaternion.RotationY(1) = \n");
        Console.WriteLine(Quaternion.RotationY(1));


        // RotationZ
        Console.Write("\nRotationZ\n");
        Console.Write("Quaternion.RotationZ(1) = \n");
        Console.WriteLine(Quaternion.RotationZ(1));


        // RotationYxz
        Console.Write("\nRotationYxz\n");
        Console.Write("Quaternion.RotationYxz(1, 2, 3) = \n");
        Console.WriteLine(Quaternion.RotationYxz(1, 2, 3));


        // ToMatrix4
        Console.Write("\nToMatrix4\n");
        q1 = Quaternion.RotationYxz(1, 2, 3);

        Console.Write("Quaternion q1 = \n");
        Console.WriteLine(q1);

        Console.Write("q1.ToMatrix4() = \n");
        Console.WriteLine(q1.ToMatrix4());


        // FromMatrix4
        Console.Write("\nFromMatrix4\n");
        m1 = Matrix4.RotationYxz(1, 2, 3);

        Console.Write("Matrix4 m1 = \n");
        Console.WriteLine(m1);

        Console.Write("Quaternion.FromMatrix4(m1) = \n");
        Console.WriteLine(Quaternion.FromMatrix4(m1));
    }
}

} // Sample
