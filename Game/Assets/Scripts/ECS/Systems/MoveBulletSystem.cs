using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(MoveBulletSystem))]
public sealed class MoveBulletSystem : UpdateSystem
{
    private Filter filter;

    public override void OnAwake()
    {
        filter = World.Filter
            .With<BulletComponent>()
            .With<PositionComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        UpdateMoving(deltaTime);
    }

    private void UpdateMoving(float deltaTime)
    {
        var positions = filter.Select<PositionComponent>();
        var bullets = filter.Select<BulletComponent>();

        for (int i = 0; i < filter.Length; i++)
        {
            ref var position = ref positions.GetComponent(i);
            ref var bullet = ref bullets.GetComponent(i);

            position.Position += bullet.Direction * bullet.Speed * deltaTime;
        }
    }
}