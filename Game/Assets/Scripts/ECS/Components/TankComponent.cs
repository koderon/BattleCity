using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct TankComponent : IComponent
{
    public bool IsNeedShot;

    public float MaxTimeToShoot;
    public float TimeToShoot;

    public Vector3 DirectionToMove;

    public void Shot()
    {
        IsNeedShot = true;
        TimeToShoot = 0;
    }
}