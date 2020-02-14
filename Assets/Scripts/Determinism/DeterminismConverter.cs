using UnityEngine;
using UnityEditor;

namespace DeterminismPhysics
{
    public class DeterminismConverter
    {

        public static int FloatToInt(float f)
        {
            return (int)(f / PhysicsEngine.instance.minPrecision);
        }

        public static float IntToFloat (int i)
        {
            return (float)(i * PhysicsEngine.instance.minPrecision);
        }
    }
}