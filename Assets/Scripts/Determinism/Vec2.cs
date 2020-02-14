using UnityEngine;
using UnityEditor;
using System;

namespace DeterminismPhysics
{
    public class Vec2
    {
        public int x;
        public int y;

        public Vec2(int x, int y)
        {
            this.x = x;
            this.y = y;

        }
        public static Vec2 Zero { get { return new Vec2(0, 0); } }
        public static Vec2 operator + (Vec2 vec , int i)
        {
            return new Vec2(vec.x + i, vec.y + i);
        }
        public static Vec2 operator +(Vec2 vec1, Vec2 vec2)
        {
            return new Vec2(vec1.x + vec2.x, vec1.y + vec2.y);
        }
        public static Vec2 operator -(Vec2 vec, int i)
        {
            return new Vec2(vec.x - i, vec.y - i);
        }
        public static Vec2 operator -(Vec2 vec1, Vec2 vec2)
        {
            return new Vec2(vec1.x - vec2.x, vec1.y - vec2.y);
        }
        public static Vec2 operator /(Vec2 vec, int i)
        {
            return new Vec2(vec.x/i, vec.y/i);
        }
        public static Vec2 operator *(Vec2 vec, int i)
        {
            return new Vec2(vec.x * i, vec.y * i);
        }
        public static Vec2 operator /(Vec2 vec, float f)
        {
            return new Vec2((int)(vec.x / f), (int)(vec.y / f));
        }
        public static Vec2 operator *(Vec2 vec, float f)
        {
            return new Vec2((int)(vec.x * f), (int)(vec.y * f));
        }

        public static Vector3 Vec2ToVector3F(Vec2 vec2)
        {
            return new Vector3(vec2.x*PhysicsEngine.instance.minPrecision, 0, vec2.y* PhysicsEngine.instance.minPrecision);
        }
        public static Vec2 Vector3FToVec2(Vector3 vec3)
        {
            return new Vec2((int)(vec3.x/ PhysicsEngine.instance.minPrecision), (int)(vec3.z/ PhysicsEngine.instance.minPrecision));
        }
        public int Magnitude()
        {
            return (int)Math.Sqrt( x^2 + y^2);
        }
        public int Distance(Vec2 vec)
        {
            return (int)Math.Sqrt((vec.x - this.x) ^ 2 + (vec.y - this.y) ^ 2);
        }
        public Vec2 MoveTowards(Vec2 targetPos , int i)
        {
            Vec2 a = targetPos - this;
            int magnitude = a.Magnitude();
            if (magnitude <= i || magnitude == 0f)
            {
                return targetPos;
            }
            return this + a / magnitude * i;

        }
    }
}
