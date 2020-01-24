// <copyright file="Camera.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020
{
    using System;
    using SharpDX;
    using SharpDX.DirectInput;

    internal class Camera
    {
        private readonly IInput input;
        private Matrix projection;
        private Matrix view;
        private Vector4 position = new Vector4(0.0f, 5.0f, 5.0f, 1.0f);

        private float horizontalAngle;
        private float verticalAngle = -0.25f;
        private long lastTime;

        public Camera(IInput input, int width, int height)
        {
            this.input = input;
            this.SetProjection(width, height);
        }

        public Matrix View => this.view;

        public Matrix Projection => this.projection;

        public void SetProjection(int width, int height)
        {
            this.projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)width / height, 0.01f, 1000.0f);
        }

        public void Update(long time)
        {
            var elapsed = time - this.lastTime;
            this.lastTime = time;

            this.UpdateViewAngles(elapsed);

            this.view = this.CalculateView(out Vector4 lookDir, out Vector3 right);

            this.Move(elapsed, lookDir, right);
        }

        private void UpdateViewAngles(long elapsed)
        {
            var mouseDelta = this.input.MousePosition();

            this.horizontalAngle += mouseDelta.X * (elapsed / 5000.0f);
            this.verticalAngle -= mouseDelta.Y * (elapsed / 5000.0f);

            if (this.verticalAngle < -1.57)
            {
                this.verticalAngle = -1.57f;
            }

            if (this.verticalAngle > 1.57)
            {
                this.verticalAngle = 1.57f;
            }
        }

        private Matrix CalculateView(out Vector4 lookDir, out Vector3 right)
        {
            lookDir = new Vector4(0.0f, 0.0f, -1.0f, 0.0f);
            lookDir = Vector4.Transform(lookDir, Matrix.RotationX(this.verticalAngle));
            lookDir = Vector4.Transform(lookDir, Matrix.RotationY(this.horizontalAngle));

            var lookDir3 = new Vector3(lookDir.X, lookDir.Y, lookDir.Z);
            var pos3 = new Vector3(this.position.X, this.position.Y, this.position.Z);

            var lookAt = pos3 + lookDir3;

            right = Vector3.Cross(new Vector3(0.0f, 1.0f, 0.0f), lookDir3);
            right.Normalize();

            var up = Vector3.Cross(lookDir3, right);

            up.Normalize();

            return Matrix.LookAtLH(pos3, lookAt, up);
        }

        private void Move(long elapsed, Vector4 lookDir, Vector3 right)
        {
            var mult = 1.0f;
            if (this.input.IsKeyDown(Key.LeftShift))
            {
                mult *= 2.0f;
            }

            if (this.input.IsKeyDown(Key.LeftControl))
            {
                mult *= 2.0f;
            }

            if (this.input.IsKeyDown(Key.W))
            {
                this.position += lookDir * mult * (elapsed / 1000.0f);
            }

            if (this.input.IsKeyDown(Key.S))
            {
                this.position -= lookDir * mult * (elapsed / 1000.0f);
            }

            if (this.input.IsKeyDown(Key.A))
            {
                this.position -= new Vector4(right, 0.0f) * mult * (elapsed / 1000.0f);
            }

            if (this.input.IsKeyDown(Key.D))
            {
                this.position += new Vector4(right, 0.0f) * mult * (elapsed / 1000.0f);
            }
        }
    }
}