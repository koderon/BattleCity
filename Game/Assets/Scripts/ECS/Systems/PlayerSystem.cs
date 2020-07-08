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
        filter = World.Filter.With<MoveComponent>().With<InputComponent>().With<TransformComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        UpdatePlayer(deltaTime);
    }

    private void UpdatePlayer(float deltaTime)
    {
        var inputs = filter.Select<InputComponent>();
        var moves = filter.Select<MoveComponent>();
        var transforms = filter.Select<TransformComponent>();

        for (int i = 0; i < filter.Length; i++)
        {
            ref var move = ref moves.GetComponent(i);
            ref var input = ref inputs.GetComponent(i);
            ref var transform = ref transforms.GetComponent(i);

            var offset = move.Speed * deltaTime;

            if(move.MoveOffset > 0 || input.Direction == Vector2.zero)
                continue;

            //Debug.Log("check " + transform.Transform.position);

            move.IsNeedCheckCollision = true;
            move.Direction = input.Direction;
            move.Rotation = move.Direction;
            move.MoveOffset = move.MaxOffset;
        }
    }
}