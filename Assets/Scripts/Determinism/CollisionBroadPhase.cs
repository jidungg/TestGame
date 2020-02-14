using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DeterminismPhysics
{
    /// <summary>
    /// 충돌 객체를 의미합니다.
    /// 
    /// </summary>
    internal class CollisionBroadPhase
    {
        //충돌한 두 객체
        public PhysicsObject A { get; private set; }
        public PhysicsObject B { get; private set; }

        //충돌 객체의 해쉬값
        private int hash;

        internal CollisionBroadPhase(PhysicsObject a, PhysicsObject b)
        {
            A = a;
            B = b;

            //make sure objects are always in the same order when calculating hash
            if (A.CompareTo(B) < 0)
            {
                hash = new Tuple<PhysicsObject, PhysicsObject>(A, B).GetHashCode();
            }
            else
            {
                hash = new Tuple<PhysicsObject, PhysicsObject>(B, A).GetHashCode();
            }
        }

        public override int GetHashCode()
        {
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is CollisionBroadPhase)
            {
                CollisionBroadPhase other = (CollisionBroadPhase)obj;
                if ((A.Equals(other.A) && B.Equals(other.B)) || (A.Equals(other.B) && B.Equals(other.A)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

