using System.Linq;
using Morpeh;
using Morpeh.Globals;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(CreateObjectSystem))]
public sealed class CreateObjectSystem : UpdateSystem
{
    public GameObject BulletPrefab;

    public GlobalEventObject CreateBulletEvent;

    private Filter tankFilter;

    public override void OnAwake()
    {
        tankFilter = World.Filter.With<TankComponent>().With<MoveUnitComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        UpdateCreateBullet();
    }

    private void UpdateCreateBullet()
    {
        if (!CreateBulletEvent.IsPublished)
            return;

        var tanks = tankFilter.Select<TankComponent>();

        var data = CreateBulletEvent.BatchedChanges;

        foreach (var o in data)
        {
            var model = (TankSystem.BulletData) o;
            /*if(model != null)
                Debug.Log("data := " + model.TextMessage);*/

            CreateBullet(model);
        }
    }

    private void CreateBullet(TankSystem.BulletData data)
    {
        var tanks = tankFilter.Select<TankComponent>();
        var moves = tankFilter.Select<MoveUnitComponent>();

        var index = tankFilter.ToList().FindIndex(g => g.ID == data.IdTank);
        var tankEntity = tankFilter.GetEntity(index);

        var tank = tanks.GetComponent(index);
        var move = moves.GetComponent(index);
        
        var bullet = Helper.Create<BulletProvider>(BulletPrefab);

        ref var tankPosition = ref tankEntity.GetComponent<PositionComponent>();
        ref var typeTank = ref tankEntity.GetComponent<ObjectTypeComponent>();

        ref var bulletComponent = ref bullet.Entity.GetComponent<BulletComponent>();
        ref var objectType = ref bullet.Entity.GetComponent<ObjectTypeComponent>();
        ref var position = ref bullet.Entity.GetComponent<PositionComponent>();

        bulletComponent.Direction = move.Rotation;
        objectType.ObjectType = typeTank.ObjectType;

        position.Position = tankPosition.Position + bulletComponent.Direction * 1.2f;
        bullet.transform.position = position.Position;
    }
}