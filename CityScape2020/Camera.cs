using System;
using SharpDX;
using SharpDX.DirectInput;

namespace CityScape2020
{
    class Camera
    {
        private readonly IInput input;
        private Matrix projection;
        private Matrix view;
        private Vector4 position = new Vector4(0.0f, 5.0f, 5.0f, 1.0f);

        private float horizontalAngle, verticalAngle = -0.25f;
        private long lastTime;

        public Camera(IInput input, int width, int height)
        {
            this.input = input;
            SetProjection(width, height);
        }

        public Matrix View => view;
        public Matrix Projection => projection;

        public void SetProjection(int width, int height)
        {
            projection = Matrix.PerspectiveFovLH((float) Math.PI/4.0f, (float) width/height, 0.01f, 1000.0f);
        }

        public void Update(long time)
        {
            var elapsed = time - lastTime;
            lastTime = time;

            var mult = 1.0f;
            if (input.IsKeyDown(Key.LeftShift))
                mult *= 2.0f;
            if (input.IsKeyDown(Key.LeftControl))
                mult *= 2.0f;

            var mouseDelta = input.MousePosition();

            horizontalAngle += mouseDelta.X*(elapsed/5000.0f);
            verticalAngle -= mouseDelta.Y*(elapsed/5000.0f);

            if (verticalAngle < -1.57) verticalAngle = -1.57f;
            if (verticalAngle > 1.57) verticalAngle = 1.57f;

            var lookDir = new Vector4(0.0f, 0.0f, -1.0f, 0.0f);
            lookDir = Vector4.Transform(lookDir, Matrix.RotationX(verticalAngle));
            lookDir = Vector4.Transform(lookDir, Matrix.RotationY(horizontalAngle));

            var lookDir3 = new Vector3(lookDir.X, lookDir.Y, lookDir.Z);
            var pos3 = new Vector3(position.X, position.Y, position.Z);

            var lookAt = pos3 + lookDir3;

            var right = Vector3.Cross(new Vector3(0.0f, 1.0f, 0.0f), lookDir3);
            right.Normalize();

            var up = Vector3.Cross(lookDir3, right);

            up.Normalize();

            view = Matrix.LookAtLH(pos3, lookAt, up);

            if (input.IsKeyDown(Key.W))
            {
                position += lookDir*mult*(elapsed/1000.0f);
            }

            if (input.IsKeyDown(Key.S))
            {
                position -= lookDir*mult*(elapsed/1000.0f);
            }
            
            if (input.IsKeyDown(Key.A))
            {
                position -= new Vector4(right, 0.0f)*mult*(elapsed/1000.0f);
            }

            if (input.IsKeyDown(Key.D))
            {
                position += new Vector4(right, 0.0f)*mult*(elapsed/1000.0f);
            }
        }

    }
}