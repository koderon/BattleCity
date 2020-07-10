using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerSystem))]
public sealed class PlayerSystem : UpdateSystem
{
    private Filter filter;

    public override void OnAwake()
    {
        filter = World.Filter.With<TankComponent>().With<InputComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        UpdatePlayer(deltaTime);
    }

    private void UpdatePlayer(float deltaTime)
    {
        var inputs = filter.Select<InputComponent>();
        var tankComponents = filter.Select<TankComponent>();

        for (int i = 0; i < filter.Length; i++)
        {
            ref var tank = ref tankComponents.GetComponent(i);
            ref var input = ref inputs.GetComponent(i);

            input.IsNeedShoot = true;
            if (input.IsNeedShoot && !tank.IsNeedShot)
                tank.Shot();
            
            tank.DirectionToMove = input.Direction;

            /*
            var offset = move.Speed * deltaTime;

            if(move.MoveOffset > 0 || input.Direction == Vector2.zero)
                continue;

            //Debug.Log("check " + transform.Transform.position);

            move.IsNeedCheckCollision = true;
            move.Direction = input.Direction;
            move.Rotation = move.Direction;
            move.MoveOffset = move.MaxOffset;*/
        }
    }
}