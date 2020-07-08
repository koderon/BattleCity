using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(AABBSystem))]
public sealed class AABBSystem : UpdateSystem
{
    private Filter filter;

    public override void OnAwake()
    {
        filter = World.Filter.With<ColliderComponent>().With<PositionComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        var collidables = filter.Select<ColliderComponent>();
        var positions = filter.Select<PositionComponent>();

        for (int i = 0; i < filter.Length; i++)
        {
            ref var collidable = ref collidables.GetComponent(i);
            ref var position = ref positions.GetComponent(i);

            collidable.Collider.BoundBox.center = position.Position;
        }
    }
}