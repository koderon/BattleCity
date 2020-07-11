using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Initializers/" + nameof(PositionInitializer))]
public sealed class PositionInitializer : Initializer
{
    private Filter filter;
    private Filter tankFilter;

    public override void OnAwake()
    {
        filter = World.Filter.With<TransformComponent>().With<PositionComponent>();
        tankFilter = World.Filter.With<TankComponent>().With<MoveUnitComponent>();

        InitPositions();
        InitTanks();
    }

    public override void Dispose() {
    }

    public void InitPositions()
    {
        var transforms = filter.Select<TransformComponent>();
        var positions = filter.Select<PositionComponent>();

        for (int i = 0; i < filter.Length; i++)
        {
            ref var transform = ref transforms.GetComponent(i);
            ref var position = ref positions.GetComponent(i);

            position.Position = transform.Transform.position;
        }
    }

    private void InitTanks()
    {
        var moves = tankFilter.Select<MoveUnitComponent>();

        for (int i = 0; i < tankFilter.Length; i++)
        {
            ref var move = ref moves.GetComponent(i);

            move.Rotation = Vector3.up;
        }

    }
}