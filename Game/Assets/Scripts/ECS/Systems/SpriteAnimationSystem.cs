using Boo.Lang.Environments;
using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(SpriteAnimationSystem))]
public sealed class SpriteAnimationSystem : UpdateSystem
{
    private Filter filter;

    public override void OnAwake()
    {
        filter = World.Filter.With<MoveUnitComponent>().With<SpriteAnimationComponent>();

        Init();
    }

    public override void OnUpdate(float deltaTime) {
        var moves = filter.Select<MoveUnitComponent>();
        var animations = filter.Select<SpriteAnimationComponent>();

        for (int i = 0; i < filter.Length; i++)
        {
            ref var move = ref moves.GetComponent(i);
            ref var animation = ref animations.GetComponent(i);

            if (move.Direction != Vector3.zero)
            {
                animation.TimeToAnimation += deltaTime;
                var timeMaxFrameInSecond = 1 / animation.MaxFrameInSecond;

                if (timeMaxFrameInSecond < animation.TimeToAnimation)
                {
                    animation.TimeToAnimation -= timeMaxFrameInSecond;
                    animation.Index++;
                    if (animation.Index >= animation.MaxFrame)
                        animation.Index = 0;
                }
            }

            GetSprite(ref animation, move.Rotation);
        }
    }

    private void GetSprite(ref SpriteAnimationComponent animation, Vector3 direction)
    {
        if (animation.CurrentDirection == direction && animation.CurrentIndex == animation.Index)
            return;

        animation.CurrentDirection = direction;
        animation.CurrentIndex = animation.Index;

        animation.Image.sprite = animation.TankSprites.GetSpriteOnDirection(animation.CurrentDirection, animation.CurrentIndex);
    }

    private void Init()
    {
        var moves = filter.Select<MoveUnitComponent>();
        var animations = filter.Select<SpriteAnimationComponent>();

        for (int i = 0; i < filter.Length; i++)
        {
            ref var move = ref moves.GetComponent(i);
            ref var animation = ref animations.GetComponent(i);

            animation.CurrentIndex = -1;
            animation.CurrentDirection = Vector3.up;
        }
    }
}