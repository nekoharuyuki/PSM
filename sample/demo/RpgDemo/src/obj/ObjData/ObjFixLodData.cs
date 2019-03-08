/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using Sce.PlayStation.Core;
using AppRpg.Data;

 namespace AppRpg {

///***************************************************************************
/// LOD�֘A�f�[�^
///***************************************************************************
public static class ObjFixLodData
{
    /// ���i�̋��E�f�[�^
    public static float[,] BoundingBox = {
        { -0.550000f,0.000000f,-0.550000f,0.550000f,1.104408f,0.550000f },
        { -0.392819f,0.000000f,-0.392819f,0.392819f,1.040090f,0.392819f },
        { -1.204083f,0.002174f,-0.102566f,1.408089f,1.314935f,0.106130f },
        { -0.686113f, -0.112371f, -0.794814f, 0.687159f, 1.015166f, 1.018355f },
        { -0.490101f,-0.000311f,-0.045017f,0.490649f,1.594048f,0.121641f },
        { -0.478540f,-0.000599f,-1.039229f,0.496702f,0.938919f,1.072867f },
        { -5.804140f,-0.000413f,-11.242024f,5.788230f,12.361773f,11.288110f },
        { -5.790672f,0.001122f,-15.218775f,12.206923f,12.390614f,7.263075f },
        { -2.483159f,0.027497f,-4.095434f,2.513247f,6.014069f,1.010146f },
        { -1.078745f,0.007007f,-1.196814f,1.077596f,1.492059f,2.357620f },
        { -0.579525f,-0.008320f,-0.884344f,0.581616f,0.949969f,0.862868f },
        { -0.785085f,0.000000f,-2.607094f,0.785085f,0.824360f,2.559800f },
        { -2.644221f,0.014789f,-0.420179f,2.723382f,5.687049f,0.614160f },
        { -6.166885f, -0.073724f, -6.166885f, 6.166885f, 10.950006f, 6.166885f },
        { -4.809242f, 0.000000f, -4.733959f, 4.590577f, 11.386863f, 4.776579f },
        { -0.599873f,0.000000f,-0.599873f,0.599873f,4.514674f,0.599873f },
        { -0.597553f,-0.000388f,-0.597552f,0.602193f,3.411914f,0.602193f },
        { -0.599873f,0.000000f,-0.599873f,0.599873f,1.879911f,0.599873f },
        { -5.657736f,0.001760f,-7.072142f,5.655571f,11.397208f,7.096339f },
        { -14.099995f,0.000000f,-9.421554f,14.060631f,11.899638f,9.075136f },
    };

    /// LOD�Ŏg�p������i�̃��f��ID
    public static ModelResId[,] LodModelTbl = {
        { ModelResId.Fix00,  ModelResId.Fix00,     ModelResId.Eff00,       ModelResId.Eff00,       ModelResId.Eff00 },     /// �ؔ�
        { ModelResId.Fix01,  ModelResId.Fix01_l1,  ModelResId.Eff00,       ModelResId.Eff00,       ModelResId.Eff00 },     /// �ؒM
        { ModelResId.Fix02,  ModelResId.Fix02_l1,  ModelResId.Eff00,       ModelResId.Eff00,       ModelResId.Eff00 },     /// �؂̍�
        { ModelResId.Fix03,  ModelResId.Fix03_l1,  ModelResId.Eff00,       ModelResId.Eff00,       ModelResId.Eff00 },     /// �Ⴂ��
        { ModelResId.Fix04,  ModelResId.Fix04_l1,  ModelResId.Eff00,       ModelResId.Eff00,       ModelResId.Eff00 },     /// ���D
        { ModelResId.Fix05,  ModelResId.Fix05_l1,  ModelResId.Fix05_l1,    ModelResId.Eff00,       ModelResId.Eff00 },     /// �Β�
        { ModelResId.Fix06,  ModelResId.Fix06,     ModelResId.Fix06,       ModelResId.Fix06_l1,    ModelResId.Fix06_l1 },  /// ��Type2
        { ModelResId.Fix07,  ModelResId.Fix07,     ModelResId.Fix07,       ModelResId.Fix07_l1,    ModelResId.Fix07_l1 },  /// ��Type3
        { ModelResId.Fix08,  ModelResId.Fix08,     ModelResId.Fix08,       ModelResId.Fix08_l1,    ModelResId.Fix08_l1 },  /// ��Type4
        { ModelResId.Fix09,  ModelResId.Fix09_l1,  ModelResId.Eff00,       ModelResId.Eff00,       ModelResId.Eff00 },     /// �׎�
        { ModelResId.Fix10,  ModelResId.Fix10_l1,  ModelResId.Eff00,       ModelResId.Eff00,       ModelResId.Eff00 },     /// �d�R
        { ModelResId.Fix11,  ModelResId.Fix11_l1,  ModelResId.Eff00,       ModelResId.Eff00,       ModelResId.Eff00 },     /// ���M
        { ModelResId.Fix12,  ModelResId.Fix12_l1,  ModelResId.Fix12_l1,    ModelResId.Eff00,       ModelResId.Eff00 },     /// �A�[�`
        { ModelResId.Fix13,  ModelResId.Fix13_l2,  ModelResId.Fix13_l2,    ModelResId.Eff00,       ModelResId.Eff00 },     /// ��Type0
        { ModelResId.Fix14,  ModelResId.Fix14_l2,  ModelResId.Fix14_l2,    ModelResId.Eff00,       ModelResId.Eff00 },     /// ��Type1
        { ModelResId.Fix15,  ModelResId.Fix15_l1,  ModelResId.Fix15_l1,    ModelResId.Eff00,       ModelResId.Eff00 },     /// �Β�Type0
        { ModelResId.Fix16,  ModelResId.Fix16_l1,  ModelResId.Fix16_l1,    ModelResId.Eff00,       ModelResId.Eff00 },     /// �Β�Type1
        { ModelResId.Fix17,  ModelResId.Fix17_l1,  ModelResId.Fix17_l1,    ModelResId.Eff00,       ModelResId.Eff00 },     /// �Β�Type2
        { ModelResId.Fix18,  ModelResId.Fix18,     ModelResId.Fix18,       ModelResId.Fix18_l1,    ModelResId.Fix18_l1 },  /// ��Type0
        { ModelResId.Fix19,  ModelResId.Fix19,     ModelResId.Fix19,       ModelResId.Fix19_l1,    ModelResId.Fix19_l1 },  /// ��Type1
    };


    /// ���E���̒��S���W��擾
    static public Vector3 GetBoundingCenterPos( int idx )
    {
        Vector3 pos = new Vector3();
        pos.X = (BoundingBox[idx, 0] + BoundingBox[idx, 3]) / 2.0f;
        pos.Y = (BoundingBox[idx, 1] + BoundingBox[idx, 4]) / 2.0f;
        pos.Z = (BoundingBox[idx, 2] + BoundingBox[idx, 5]) / 2.0f;
        return pos;
    }


    /// ���E���̔��a��擾
    static public float GetBoundingCenterWidth( int idx )
    {
        Vector3 pos1 = new Vector3( BoundingBox[idx, 0], BoundingBox[idx, 1], BoundingBox[idx, 2] );
        Vector3 pos2 = new Vector3( BoundingBox[idx, 3], BoundingBox[idx, 4], BoundingBox[idx, 5] );
        float r = Common.VectorUtil.Distance( pos1, pos2 )/ 2.0f;
        return r;
    }

}
} // namespace
