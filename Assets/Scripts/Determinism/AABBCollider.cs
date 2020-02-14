using UnityEngine;
using System.Collections;

namespace DeterminismPhysics
{
    public class AABBCollider : MonoBehaviour, ICollider
    {
        public bool UseCurrentPos =false;
        public int width;
        public int height;

        public Vec2 min;
        public Vec2 max;

        public void Awake()
        {
            if (UseCurrentPos)
            {
                min = new Vec2(DeterminismConverter.FloatToInt(transform.position.x) - width / 2, DeterminismConverter.FloatToInt(transform.position.z) - height / 2);
                max = new Vec2(DeterminismConverter.FloatToInt(transform.position.x) + width / 2, DeterminismConverter.FloatToInt(transform.position.z) + height / 2);
            }
            else
            {
                min = new Vec2(- width / 2,- height / 2);
                max = new Vec2(+width / 2, +height / 2);
            }
        }
        public void Initiallize(Vec2 min, Vec2 max)
        {
            if (!UseCurrentPos)
            {
                this.min = min;
                this.max = max;
            }

            if (Width % 2 != 0)
            {
                this.max.x++;
            }
            if (Height % 2 != 0)
            {
                this.max.y++;
            }


        }


        public int Width
        {
            get => max.x - min.x;
        }

        public int Height
        {
            get => max.y - min.y;
        }

        public void Update()
        {
            this.transform.position = Vec2.Vec2ToVector3F(GetCenter());
            Debug.DrawRay(Vec2.Vec2ToVector3F(min), transform.TransformDirection(Vector3.forward) * DeterminismConverter.IntToFloat(Height), Color.red);
            Debug.DrawRay(Vec2.Vec2ToVector3F(min), transform.TransformDirection(Vector3.right) * DeterminismConverter.IntToFloat(Width), Color.red);
            Debug.DrawRay(Vec2.Vec2ToVector3F(max), transform.TransformDirection(Vector3.back) * DeterminismConverter.IntToFloat(Height), Color.red);
            Debug.DrawRay(Vec2.Vec2ToVector3F(max), transform.TransformDirection(Vector3.left) * DeterminismConverter.IntToFloat(Width), Color.red);

        }
        public Vec2 GetCenter()
        {
            return new Vec2((min.x + max.x) / 2, (min.y + max.y) / 2);
        }
        public bool IsIntersects(ICollider col)
        {
            if (col is AABBCollider)
            {
                AABBCollider aabb = (AABBCollider)col;
                if (this.max.x < aabb.min.x || this.min.x > aabb.max.x) { return false; }
                if (this.max.y < aabb.min.y || this.min.y > aabb.max.y) { return false; }
                return true;
            }
            else if (col is CircleCollider)
            {
                CircleCollider circle = (CircleCollider)col;
                if (min.x > (circle.center.x + circle.radius) || max.x < (circle.center.x - circle.radius)) { return false; }
                if (min.y > (circle.center.y + circle.radius) || max.y < (circle.center.y - circle.radius)) { return false; }
                return true;

            }
            else
            {
                return false;
            }
        }
        public bool IsContains(ICollider col)
        {
            if (col is AABBCollider)
            {
                AABBCollider aabb = (AABBCollider)col;
                if (aabb.min.x < this.min.x) { return false; }
                if (aabb.max.x > this.max.x) { return false; }
                if (aabb.min.y < this.min.y) { return false; }
                if (aabb.max.y > this.max.y) { return false; }

                return true;
            }
            else if (col is CircleCollider)
            {
                CircleCollider circle = (CircleCollider)col;
                if ((circle.center.x - circle.radius) < min.x) { return false; }
                if ((circle.center.x + circle.radius) > max.x) { return false; }
                if ((circle.center.y - circle.radius) < min.y) { return false; }
                if ((circle.center.y + circle.radius) < max.y) { return false; }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void MoveTowards(Vec2 targetPos, int i)
        {
            Vec2 a = targetPos - GetCenter();
            int magnitude = a.Magnitude();
            if (magnitude <= i || magnitude == 0f)
            {
                max = targetPos + new Vec2(Width / 2, Height / 2);
                min = targetPos - new Vec2(Width / 2, Height / 2);
                return;
            }

            min += a / magnitude * i;
            max += a / magnitude * i;
        }


    }
}

