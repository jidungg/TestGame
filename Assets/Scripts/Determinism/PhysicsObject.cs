using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DeterminismPhysics
{
    public class PhysicsObject : MonoBehaviour
    {
        public enum ColliderType { AABB, Circle }

        public int MassGWN;
        public Vec2 Velocity;
        public static int GlobalID =0;
        public int id;
        public ColliderType type;
        public ICollider DCollider;

        public void Awake()
        {
            switch (type)
            {
                case ColliderType.AABB:
                    DCollider = gameObject.GetComponent<AABBCollider>();
                    break;

                case ColliderType.Circle:
                    DCollider = gameObject.GetComponent<CircleCollider>();
                    break;

                default:
                    break;
            }
            id = GlobalID++;

        }
        public void Start()
        {
            PhysicsEngine.instance.InsertObject(this);
        }
        public void OnDisable()
        {
            PhysicsEngine.instance.RemoveObject(this);
        }
        
        public int CompareTo(PhysicsObject b)
        {
            return this.id - b.id;
        }
        void Reposition()
        {
            transform.position = Vec2.Vec2ToVector3F(DCollider.GetCenter());
        }
        public void MoveTowards(Transform target)
        {
            StartCoroutine(MoveCoroutine(target));
        }
        IEnumerator MoveCoroutine(Transform target)
        {
            Vector3 distFromTarget = this.transform.position - target.position;
            while (distFromTarget.sqrMagnitude > 1)
            {
                

                Reposition();
                distFromTarget = this.transform.position - target.position;
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public interface ICollider 
    {
        bool IsIntersects(ICollider col);
        Vec2 GetCenter();
        void MoveTowards(Vec2 targetPos , int i);
    }


  


}
