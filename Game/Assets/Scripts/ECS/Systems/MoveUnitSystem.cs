using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(MoveUnitSystem))]
public sealed class MoveUnitSystem : UpdateSystem
{
    private Filter filter;
    private Filter colliderFilter;

    public override void OnAwake()
    {
        filter = World.Filter
            .With<MoveUnitComponent>()
            .With<PositionComponent>()
            .With<ColliderComponent>();

        colliderFilter = World.Filter.With<ColliderComponent>().Without<BulletComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        UpdateMoving(deltaTime);
    }

    private void UpdateMoving(float deltaTime)
    {
        var positions = filter.Select<PositionComponent>();
        var moves = filter.Select<MoveUnitComponent>();
        //var colliders = filter.Select<ColliderComponent>();

       
        for (int i = 0; i < filter.Length; i++)
        {
            ref var position = ref positions.GetComponent(i);
            ref var move = ref moves.GetComponent(i);
            //ref var collider = ref colliders.GetComponent(i);
            //var id = filter.GetEntity(i).ID;

            if (move.MoveOffset <= 0 || move.IsNeedCheckCollision)
                continue;

            //CheckCollision(id, ref move, ref collider);

            var offset = deltaTime * move.Speed;
            var direction = move.Direction;
            move.MoveOffset -= offset;
            if (move.MoveOffset < 0)
            {
                offset += move.MoveOffset;
                ResetMove(ref move);
            }

            position.Position += direction * offset;
        }
    }

    private void CheckCollision(int id, ref MoveUnitComponent move, ref ColliderComponent colliderComponent)
    {
        if (!move.IsNeedCheckCollision)
            return;

        move.IsNeedCheckCollision = false; 

        var colliders = colliderFilter.Select<ColliderComponent>();

        colliderComponent.Collider.BoundBox.center += move.Direction * move.MaxOffset;
        
        for (int i = 0; i < colliderFilter.Length; i++)
        {
            ref var collider = ref colliders.GetComponent(i);
            var Id = colliderFilter.GetEntity(i).ID;

            if(Id == id)
                continue;

            if (colliderComponent.Collider.BoundBox.Intersects(collider.Collider.BoundBox))
            {
                ResetMove(ref move);
                break;
            }
        }
    }

    private void ResetMove(ref MoveUnitComponent move)
    {
        move.MoveOffset = 0;
        move.Direction = Vector3.zero;
    }
}