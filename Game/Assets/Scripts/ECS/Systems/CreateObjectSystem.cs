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
    public GlobalEventObject CreateBulletEvent;

    private Filter filter;

    public override void OnAwake() {
    }

    public override void OnUpdate(float deltaTime) {
        UpdateCreateBullet();
    }

    public void UpdateCreateBullet()
    {
        if (!CreateBulletEvent.IsPublished)
            return;

        var data = CreateBulletEvent.BatchedChanges;

        foreach (var o in data)
        {
            var model = (TankSystem.BulletData) o;
            if(model != null)
                Debug.Log("data := " + model.TextMessage);
        }
    }
}