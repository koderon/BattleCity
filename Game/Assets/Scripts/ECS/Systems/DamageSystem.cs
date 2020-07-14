using System.Collections.Generic;
using System.Linq;
using Morpeh;
using Morpeh.Globals;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(DamageSystem))]
public sealed class DamageSystem : UpdateSystem
{
    public GlobalEventObject BulletCollisionEvent;

    private Filter filter;

    public override void OnAwake()
    {
        filter = World.Filter.With<ColliderComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        CheckBulletCollisionEvent();
    }

    private void CheckBulletCollisionEvent()
    {
        if (!BulletCollisionEvent.IsPublished)
            return;

        foreach (var data in BulletCollisionEvent.BatchedChanges)
        {
            var bulletCollisionData = (CheckCollisionSystem.BulletCollisionData) data;

            var bulletEntity = GetEntity(bulletCollisionData.BulletId);
            if(bulletEntity == null)
                continue;

            SetDamageToObject(bulletEntity, bulletCollisionData.EncounteredIds);
            
            Debug.Log("BulletId := " + bulletCollisionData.BulletId + " encounters count := " + bulletCollisionData.EncounteredIds.Count);

        }
    }

    private void SetDamageToObject(IEntity bulletEntity, List<int> EncounteredIds)
    {
        var bulletComponent = DeactivateBullet(bulletEntity);

        foreach (var id in EncounteredIds)
        {
            var entity = GetEntity(id);
            if(entity == null)
                continue;

            var type = entity.GetComponent<ObjectTypeComponent>();
            if (type.ObjectType == EObjectType.Brick)
                SetBrickDamage(entity, bulletComponent.Damage);
            
            if((type.ObjectType == EObjectType.EnemyTank && bulletComponent.ObjectType != EObjectType.EnemyTank) ||
                type.ObjectType == EObjectType.FriendTank && bulletComponent.ObjectType != EObjectType.FriendTank)
                SetTankDamage(entity, bulletComponent.Damage);

            if (type.ObjectType == EObjectType.Bullet)
                DeactivateBullet(entity);
        }
    }

    private void SetTankDamage(IEntity tankEntity, float damagePoint)
    {
        ref var healthComponent = ref tankEntity.GetComponent<HealthComponent>();
        healthComponent.HealthPoint -= damagePoint;
    }

    private void SetBrickDamage(IEntity brickEntity, float damagePoint)
    {
        ref var healthComponent = ref brickEntity.GetComponent<HealthComponent>();
        healthComponent.HealthPoint -= damagePoint;
    }

    private BulletComponent DeactivateBullet(IEntity bulletEntity)
    {
        ref var bulletComponent = ref bulletEntity.GetComponent<BulletComponent>();
        ref var positionComponent = ref bulletEntity.GetComponent<PositionComponent>();

        ref var healthComponent = ref bulletEntity.GetComponent<HealthComponent>();
        healthComponent.HealthPoint -= 1;

        
        bulletComponent.Direction = Vector3.zero;
        positionComponent.Position = new Vector3(10000, 10000, 0);
        return bulletComponent;
    }

    private IEntity GetEntity(int id)
    {
        var index = filter.ToList().FindIndex(g => g.ID == id);
        if (index == -1)
            return null;
        var entity = filter.GetEntity(index);
        return entity;
    }
}