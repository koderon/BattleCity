using System;
using Morpeh;
using Morpeh.Globals;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Object = UnityEngine.Object;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(TankSystem))]
public sealed class TankSystem : UpdateSystem
{
    public GlobalEventObject CreateBulletEvent;

    private Filter filter;

    public override void OnAwake()
    {
        filter = World.Filter.With<MoveUnitComponent>().With<TankComponent>();
    }

    public override void OnUpdate(float deltaTime) {

        var moveUnitComponents = filter.Select<MoveUnitComponent>();
        var tanks = filter.Select<TankComponent>();

        for (int i = 0; i < filter.Length; i++)
        {
            ref var move = ref moveUnitComponents.GetComponent(i);
            ref var tank = ref tanks.GetComponent(i);
            var id = filter.GetEntity(i).ID;

            if (tank.IsNeedShot)
            {
                tank.TimeToShoot += deltaTime;
                if (tank.TimeToShoot >= tank.MaxTimeToShoot)
                {
                    tank.IsNeedShot = false;

                    CreateEventToCreateBullet(id);
                }
            }

            if(move.MoveOffset > 0 || 
               tank.DirectionToMove == Vector3.zero || 
               move.IsNeedCheckCollision)
                continue;

            move.IsNeedCheckCollision = true;
            move.Direction = tank.DirectionToMove;
            move.Rotation = move.Direction;
            move.MoveOffset = move.MaxOffset;
        }
    }

    public class BulletData : ScriptableObject
    {
        public int IdTank;
        public String TextMessage;
    }

    public void CreateEventToCreateBullet(int id)
    {
        var bulletData = new BulletData();
        bulletData.IdTank = id;
        bulletData.TextMessage = "Go To Shoot!!!!";
        
        CreateBulletEvent.NextFrame(bulletData);
    }
}