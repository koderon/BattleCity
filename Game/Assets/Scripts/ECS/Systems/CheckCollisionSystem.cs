using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(CheckCollisionSystem))]
public sealed class CheckCollisionSystem : UpdateSystem
{
    private Filter unitFilter;

    private Filter allСollidersExceptBullet;

    private Filter bulletFilter;
    private Filter AllCollidersFilter;

    public override void OnAwake()
    {
        unitFilter = World.Filter.With<ColliderComponent>().With<MoveUnitComponent>();

        allСollidersExceptBullet = World.Filter.With<ColliderComponent>().Without<BulletComponent>();

        bulletFilter = World.Filter.With<BulletComponent>().With<ColliderComponent>();
        AllCollidersFilter = World.Filter.With<ColliderComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        CheckAllUnitCollision();
        CheckAllBullets();
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

    private void CheckAllBullets()
    {
        var colliders = bulletFilter.Select<ColliderComponent>();

        var all = AllCollidersFilter.Select<ColliderComponent>();

        for (int i = 0; i < bulletFilter.Length; i++)
        {
            ref var bulletCollider = ref colliders.GetComponent(i);
            var Id = bulletFilter.GetEntity(i).ID;

            for (int j = 0; j < AllCollidersFilter.Length; j++)
            {
                ref var checkCollider = ref all.GetComponent(j);
                var checkId = AllCollidersFilter.GetEntity(j).ID;

                if(checkId == Id)
                    continue;

                if (checkCollider.Collider.BoundBox.Intersects(bulletCollider.Collider.BoundBox)) 
                {
                    Collision(bulletFilter.GetEntity(i));
                    break;
                }
            }
        }
    }

    private void Collision(IEntity entity)
    {
        Debug.Log("Coolision!!!!!!!!");

        ref var transformComponent = ref entity.GetComponent<TransformComponent>();
        transformComponent.Transform.gameObject.SetActive(false);
    }

    #endregion
}