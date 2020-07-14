using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(DestroySystem))]
public sealed class DestroySystem : UpdateSystem
{
    private Filter filter;

    public override void OnAwake()
    {
        filter = World.Filter.With<HealthComponent>().With<TransformComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        DestroyComponents();
    }

    private void DestroyComponents()
    {
        var healths = filter.Select<HealthComponent>();
        var transforms = filter.Select<TransformComponent>();

        for (int i = 0; i < filter.Length; i++)
        {
            ref var health = ref healths.GetComponent(i);
            ref var t = ref transforms.GetComponent(i);

            if (health.HealthPoint <= 0)
            {
                Destroy(t.Transform.gameObject);
            }
        }
    }
}