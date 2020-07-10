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

    public override void OnAwake()
    {
        unitFilter = World.Filter.With<ColliderComponent>().With<MoveUnitComponent>();

        allСollidersExceptBullet = World.Filter.With<ColliderComponent>().Without<BulletComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        CheckAllUnitCollision();
    }

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
}