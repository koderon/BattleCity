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
        filter = World.Filter.
            With<TransformComponent>().
            With<PositionComponent>().
            With<MoveComponent>();
    }


    public override void OnUpdate(float deltaTime) {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        var positions = filter.Select<PositionComponent>();
        var transforms = filter.Select<TransformComponent>();
        var directions = filter.Select<MoveComponent>();

        for (int i = 0; i < filter.Length; i++)
        {
            ref var position = ref positions.GetComponent(i);
            ref var transform = ref transforms.GetComponent(i);
            ref var direction = ref directions.GetComponent(i);

            transform.Transform.position = position.Position;
            //var angle = Mathf.Atan2(direction.Direction.y, direction.Direction.x) * Mathf.Rad2Deg; 
           // transform.Transform.localEulerAngles = new Vector3(0, 0, angle);
        }
    }
}