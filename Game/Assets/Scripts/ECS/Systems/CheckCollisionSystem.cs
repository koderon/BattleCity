using System.Collections.Generic;
using Morpeh;
using Morpeh.Globals;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(CheckCollisionSystem))]
public sealed class CheckCollisionSystem : UpdateSystem
{
    public GlobalEventObject BulletCollisionEvent;

    private Filter unitFilter;

    private Filter allСollidersExceptBullet;

    private Filter bulletFilter;
    private Filter AllCollidersFilter;

    private const float maxTimerForBulletCollision = 0.05f;
    private float timerForBulletCollision;

    public override void OnAwake()
    {
        unitFilter = World.Filter.With<ColliderComponent>().With<MoveUnitComponent>();

        allСollidersExceptBullet = World.Filter.With<ColliderComponent>().Without<BulletComponent>();

        bulletFilter = World.Filter.With<BulletComponent>().With<ColliderComponent>();
        AllCollidersFilter = World.Filter.With<ColliderComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        CheckAllUnitCollision();
        CheckAllBullets(deltaTime);
    }

    #region Units
    
    public void CheckAllUnitCollision()
    {
        var moves = unitFilter.Select<MoveUnitComponent>();
        var unitsCollider = unitFilter.Select<ColliderComponent>();
        var all = allСollidersExceptBullet.Select<ColliderComponent>();

        for (int i = 0; i < unitFilter.Length; i++)
        {
            ref var move = ref moves.GetComponent(i);
            ref var collider = ref unitsCollider.GetComponent(i);
            var id = unitFilter.GetEntity(i).ID;

            CheckUnitCollision(id, ref move, ref collider);
        }
    }

    private void CheckUnitCollision(int id, ref MoveUnitComponent move, ref ColliderComponent colliderComponent)
    {
        if (!move.IsNeedCheckCollision)
            return;

        move.IsNeedCheckCollision = false;

        var allCollider = allСollidersExceptBullet.Select<ColliderComponent>();

        colliderComponent.Collider.BoundBox.center += move.Direction * move.MaxOffset;

        for (int i = 0; i < allСollidersExceptBullet.Length; i++)
        {
            ref var collider = ref allCollider.GetComponent(i);
            var Id = allСollidersExceptBullet.GetEntity(i).ID;

            if (Id == id)
                continue;

            if (colliderComponent.Collider.BoundBox.Intersects(collider.Collider.BoundBox))
            {
                move.MoveOffset = 0;
                move.Direction = Vector3.zero;
                break;
            }
        }
    }

    #endregion

    #region Bullet

    private void CheckAllBullets(float deltaTime)
    {
        timerForBulletCollision += deltaTime;
        if (timerForBulletCollision < maxTimerForBulletCollision)
            return;

        timerForBulletCollision = 0;


        var bulletsCollider = bulletFilter.Select<ColliderComponent>();
        var bulletComponents = bulletFilter.Select<BulletComponent>();

        var all = AllCollidersFilter.Select<ColliderComponent>();

        var encounteredIds = new List<int>(10);

        for (int i = 0; i < bulletFilter.Length; i++)
        {
            ref var bulletCollider = ref bulletsCollider.GetComponent(i);
            ref var bulletComponent = ref bulletComponents.GetComponent(i);
            var bulletId = bulletFilter.GetEntity(i).ID;

            if(bulletComponent.Direction == Vector3.zero)
                continue;

            encounteredIds.Clear();

            for (int j = 0; j < AllCollidersFilter.Length; j++)
            {
                ref var checkCollider = ref all.GetComponent(j);
                var colliderId = AllCollidersFilter.GetEntity(j).ID;

                if(colliderId == bulletId)
                    continue;
                
                if(!checkCollider.Collider.transform.gameObject.activeSelf)
                    continue;

                if (checkCollider.Collider.BoundBox.Intersects(bulletCollider.Collider.BoundBox)) 
                {
                    encounteredIds.Add(colliderId);
                    //Collision(bulletFilter.GetEntity(i));
                }
            }

            if (encounteredIds.Count > 0)
            {
                CreateCollisionEvent(bulletId, encounteredIds);
            }
        }
    }

    public class BulletCollisionData : ScriptableObject
    {
        public int BulletId;

        public List<int> EncounteredIds = new List<int>();
    }

    private void CreateCollisionEvent(int bulletId, List<int> encounteredIds)
    {
        var bulletCollisionData = new BulletCollisionData();
        bulletCollisionData.BulletId = bulletId;
        bulletCollisionData.EncounteredIds.AddRange(encounteredIds);

        BulletCollisionEvent.NextFrame(bulletCollisionData);
    }

    private void Collision(IEntity entity)
    {
        Debug.Log("Coolision!!!!!!!!");

        ref var transformComponent = ref entity.GetComponent<TransformComponent>();
        transformComponent.Transform.gameObject.SetActive(false);
    }

    #endregion
}