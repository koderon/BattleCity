using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(UpdatePositionSystem))]
public sealed class UpdatePositionSystem : UpdateSystem
{
    private Filter filter;

    public override void OnAwake()
    {
        filter = World.Filter.With<TransformComponent>().With<PositionComponent>();
    }


    public override void OnUpdate(float deltaTime) {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        var positions = filter.Select<PositionComponent>();
        var transforms = filter.Select<TransformComponent>();

        for (int i = 0; i < filter.Length; i++)
        {
            ref var position = ref positions.GetComponent(i);
            ref var transform = ref transforms.GetComponent(i);

            transform.Transform.position = position.Position;
        }
    }
}