using UnityEngine;
using System.Collections;

namespace DeterminismPhysics
{
    [RequireComponent(typeof(PhysicsObject))]
    public class CircleCollider : MonoBehaviour, ICollider
    {
        public int Radius;
        public Vec2 center;
        public int radius;

        public void Awake()
        {
            
        }
        public void Update()
        {
        }
        public CircleCollider(Vec2 center, int radius)
        {
            this.center = center;
            this.radius = radius;
        }
        public Vec2 GetCenter()
        {
            return center;
        }
        public bool IsIntersects(ICollider col)
        {
            if (col is AABBCollider)
            {
                AABBCollider aabb = (AABBCollider)col;
                if (aabb.min.x < (center.x - radius)) { return false; }
                if (aabb.max.x > (center.x + radius)) { return false; }
                if (aabb.min.y < (center.y - radius)) { return false; }
                if (aabb.max.y > (center.y + radius)) { return false; }

                return true;
            }
            else if (col is CircleCollider)
            {
                CircleCollider circle = (CircleCollider)col;
                if (center.Distance(circle.center) > ((radius + circle.radius) ^ 2)) { return false; }
                else { return true; }

            }
            else
            {
                return false;
            }
        }


        public void MoveTowards(Vec2 targetPos, int i)
        {
            center.MoveTowards(targetPos, i);
        }

    }
}

