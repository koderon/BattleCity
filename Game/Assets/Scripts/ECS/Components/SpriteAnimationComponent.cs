using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct SpriteAnimationComponent : IComponent
{
    public float MaxFrameInSecond;
    public float TimeToAnimation;

    public int MaxFrame;
    public int Index;

    public int CurrentIndex;
    public Vector3 CurrentDirection;

    public SpriteRenderer Image;

    public TankSpritesScriptableObject TankSprites;
}