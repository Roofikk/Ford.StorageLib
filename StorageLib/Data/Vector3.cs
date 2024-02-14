using System;

namespace Ford.SaveSystem.Data
{
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public readonly float Magnitude => (float)Math.Sqrt(x * x + y * y + z * z);

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
