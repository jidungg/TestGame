using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeterminismPhysics
{
    class QuadNode :MonoBehaviour
    {
        internal AABBCollider aabb;

        internal bool isLeaf;
        internal QuadNode[] childNodes;
        private List<PhysicsObject> includedObjects;

        internal void Initiallize(Vec2 min, Vec2 max)
        {
            aabb = gameObject.AddComponent<AABBCollider>();
            aabb.Initiallize(min,max);
            isLeaf = true;
            includedObjects = new List<PhysicsObject>();
        }

        internal void Subdivide()
        {
            childNodes = new QuadNode[4];
            int centerX = (aabb.min.x + aabb.max.x) / 2;
            int centerY = (aabb.min.y + aabb.max.y) / 2;
            for(int i =0; i<4; i++)
            {
                GameObject obj = new GameObject(this.name +"-"+i);
                obj.transform.parent = this.transform;
                childNodes[i] =  obj.AddComponent<QuadNode>();
            }
            childNodes[0].Initiallize(new Vec2(aabb.min.x, centerY), new Vec2(centerX, aabb.max.y));
            childNodes[1].Initiallize(new Vec2(centerX, centerY), new Vec2(aabb.max.x, aabb.max.y));
            childNodes[2].Initiallize(new Vec2(aabb.min.x, aabb.min.y), new Vec2(centerX, centerY));
            childNodes[3].Initiallize(new Vec2(centerX, aabb.min.y), new Vec2(aabb.max.x, centerY));

            isLeaf = false;
        }

        internal bool InsertObject(PhysicsObject obj)
        {
            if (!aabb.IsIntersects(obj.DCollider))
            {
                return false;
            }
            if (isLeaf)
            {
                if (includedObjects.Count < PhysicsEngine.instance.QuadSubdivideSize ||
                    ((aabb.Width / 2) < PhysicsEngine.instance.MinSizeQuad && (aabb.Height / 2) < PhysicsEngine.instance.MinSizeQuad)
                    )
                {
                    includedObjects.Add(obj);
                    return true;
                }
                Subdivide();
            }

            for (int i = 0; i < childNodes.Length; i++)
            {
                childNodes[i].InsertObject(obj);
            }
            return true;
        }

        internal void RemoveObject(PhysicsObject obj)
        {
            if (aabb.IsIntersects(obj.DCollider)) 
            {
                if (isLeaf)
                {
                    includedObjects.Remove(obj);
                }
                else
                {
                    foreach(QuadNode node in childNodes)
                    {
                        node.RemoveObject(obj);
                    }
                }
            }

        }

        internal OrderedSet<CollisionBroadPhase> CollisionInspection(PhysicsObject col)
        {
            Debug.Log("Check "+this.name);
            OrderedSet<CollisionBroadPhase> set = new OrderedSet<CollisionBroadPhase>();
            if (!this.aabb.IsIntersects(col.DCollider))
            {
                Debug.Log("CollisionInpection Returns null");
                return set;
            }
            if (!isLeaf)
            {
                foreach (QuadNode node in childNodes)
                {
                    set.Add(node.CollisionInspection(col)) ;

                }
            }
            else
            {
                foreach(PhysicsObject obj in includedObjects)
                {
                    if(obj.id == col.id) { continue; }
                    if (obj.DCollider.IsIntersects(col.DCollider))
                    {
                        CollisionBroadPhase tempCollision = new CollisionBroadPhase(col, obj);
                        set.Add(tempCollision);
                    }
                }
            }
            Debug.Log("CollisionInpection Returns : " + set);
            return set;

        }

    }
    public class QuadTree : MonoBehaviour
    {

        QuadNode rootNode;
        int minSize;
        internal List<PhysicsObject> objectList;

        internal void Initiallize()
        {
            GameObject obj = new GameObject("rootNode");
            obj.transform.parent = this.transform;
            rootNode = obj.AddComponent<QuadNode>();
            rootNode.Initiallize(new Vec2(PhysicsEngine.instance.MinXPositionInt, PhysicsEngine.instance.MinYPositionInt),
                                    new Vec2(PhysicsEngine.instance.MaxXPositionInt, PhysicsEngine.instance.MaxYPositionInt));
            minSize = PhysicsEngine.instance.MinSizeQuad;
            objectList = new List<PhysicsObject>();
        }
        
        internal void InsertObject(PhysicsObject obj)
        {
            rootNode.InsertObject(obj);
        }
        internal void RemoveObject(PhysicsObject obj)
        {
            rootNode.RemoveObject(obj);
        }

        internal void InsertObjectPermanent(PhysicsObject obj)
        {
            if (rootNode.InsertObject(obj))
            {
                objectList.Add(obj);
            }
        }
        internal void RemoveObjectPermanent(PhysicsObject obj)
        {
            rootNode.RemoveObject(obj);
            objectList.Remove(obj);
        }

        internal OrderedSet<CollisionBroadPhase> GetAllCollisions()
        {
            OrderedSet<CollisionBroadPhase> set = new OrderedSet<CollisionBroadPhase>();
            Debug.Log("GetAllCollisions started");
            foreach(PhysicsObject obj in objectList)
            {
                Debug.Log("Get Collisions about "+obj);
                set.Add( rootNode.CollisionInspection(obj));
                Debug.Log("Collision Inspection completed about " + obj);
            }
            Debug.Log("GetAllCollisions ended");
            
            return set;
        }
    }
}

