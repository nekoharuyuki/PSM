/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sample
{

class Camera
{
    Matrix4 interest;     ///< The target coordinate system (pivot/center of interest)
    Matrix4 projection;   ///< Projection matrix
    Matrix4 posture;      ///< Camera's coordinate system
    Matrix4 view;         ///< LookAt matrix ( = posture's inverse)

    public Camera( float fov, float aspect, float near, float far )
    {
        projection = Matrix4.Perspective( fov, aspect, near, far );

        posture = Matrix4.Translation( new Vector3( 0.0f, 5.0f, 30.0f ) );
        view = posture.Inverse();

        interest = Matrix4.Identity;
    }

    public void OnRotate(
                         float rx, ///< Rotation angle around the X axis of the target coordinate system, in radians
                         float ry  ///< Rotation angle around the Y axis of the target coordinate system, in radians
                         )
    {
        // Form rotation matrices
        Matrix4 rotX = Matrix4.RotationX( rx );
        Matrix4 rotY = Matrix4.RotationY( ry );

        // Target's coordinate system
        Matrix4 at = interest.Inverse();

        // Express the camera location in the target's coordinate system
        Matrix4 localCamera = at * posture;

        // Extract rotation and translation components from the camera
        Matrix4 rotCamera = localCamera;
        rotCamera.M41 = 0.0f;
        rotCamera.M42 = 0.0f;
        rotCamera.M43 = 0.0f;

        Matrix4 rotCameraInv = rotCamera.Inverse();

        /* Assume C = RT, we get T by doing R^-1C = T 
         * This is how we get the amount of translation in target's basis
         * 
         * C = Camera matrix
         * R = Camera's rotation component (orientation)
         * T = Camera's translation component (position)
         */
        Matrix4 trans = rotCameraInv * localCamera;

        posture = interest * rotY * rotCamera * rotX * trans;
        view = posture.Inverse();
    }

    public Matrix4 Projection
    {
        get{ return projection; }
    }

    public Matrix4 View
    {
        get{ return view; }
    }

    public Vector3 Position
    {
        get{ return new Vector3( posture.M41, posture.M42, posture.M43 ); }
    }
}

} // end ns Sample
