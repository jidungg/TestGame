using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeterminismPhysics
{
    public class PhysicsEngine : MonoBehaviour
    {


        public static PhysicsEngine instance;

        //쿼드트리
        QuadTree quadTree;
        public const int GameWorldScale = 100;
        //마찰
        [SerializeField]
        int Friction = 0;
        //위치 최소단위
        public float minPrecision = 0.1f;
        /// <summary>
        /// 가장 작은 쿼드의 크기
        /// </summary>
        [SerializeField]
        float minSizeQuad = 1;

        public int MinSizeQuad
        {
            get => DeterminismConverter.FloatToInt(minSizeQuad);
        }
        /// <summary>
        /// 쿼드 분리 시 최대 포함가능한 오브젝트 수
        /// </summary>
        [SerializeField]
        public int QuadSubdivideSize=3;

        //맵 최대 크기
        [SerializeField]
        float maxXPosition = 10;
        [SerializeField]
        float maxYPosition = 35;
        [SerializeField]
        float minXPosition = -10;
        [SerializeField]
        float minYPosition = -35;

        public int MaxXPositionInt
        {
            get => DeterminismConverter. FloatToInt(maxXPosition);
        }
        public int MaxYPositionInt
        {
            get => DeterminismConverter.FloatToInt(maxYPosition);
        }
        public int MinXPositionInt
        {
            get => DeterminismConverter.FloatToInt(minXPosition);
        }
        public int MinYPositionInt
        {
            get => DeterminismConverter.FloatToInt(minYPosition);
        }
        //초당 고정 프레임
        public int FixedFramesPerSecond=30;



        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != null)
            {
                Destroy(gameObject);
            }
            GameObject obj = new GameObject("QuadTree");
            obj.transform.parent = this.transform;
            quadTree=  obj.AddComponent<QuadTree>();
            quadTree.Initiallize();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        void Update()
        {
            //모든 충돌들 모음
            OrderedSet<CollisionBroadPhase> broadCollisions = quadTree.GetAllCollisions();

            foreach (CollisionBroadPhase broadCollision in broadCollisions)
            {
                //모든 충돌들 해결
                CollisionResolver.ResolveCollision(broadCollision);
            }

            foreach (PhysicsObject physicsObject in quadTree.objectList)
            {
                if (physicsObject.MassGWN > 0 && physicsObject.Velocity != Vec2.Zero)
                {
                    //remove from quad tree as it's position is about to change
                    quadTree.RemoveObject(physicsObject);
                    if(physicsObject.DCollider is AABBCollider)
                    {
                        AABBCollider aabb = (AABBCollider)physicsObject.DCollider;
                        aabb.min = aabb.min +(new Vec2(physicsObject.Velocity.x / FixedFramesPerSecond,
                                            physicsObject.Velocity.y / FixedFramesPerSecond));
                        aabb.max = aabb.max + (new Vec2(physicsObject.Velocity.x / FixedFramesPerSecond,
                                             physicsObject.Velocity.y / FixedFramesPerSecond));
                    }
                    else if(physicsObject.DCollider is CircleCollider)
                    {
                        CircleCollider circle = (CircleCollider)physicsObject.DCollider;
                        circle.center = circle.center + (new Vec2(physicsObject.Velocity.x / FixedFramesPerSecond,
                                                       physicsObject.Velocity.y / FixedFramesPerSecond));
                    }


                    //apply friction
                    physicsObject.Velocity = physicsObject.Velocity.MoveTowards(Vec2.Zero,Friction / FixedFramesPerSecond);

                    quadTree.InsertObject(physicsObject);
                }
            }
        }
        public void InsertObject(PhysicsObject obj)
        {
            quadTree.InsertObjectPermanent(obj);
        }
        public void RemoveObject(PhysicsObject obj)
        {
            quadTree.RemoveObjectPermanent(obj);
        }

    }
}


