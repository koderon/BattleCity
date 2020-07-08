using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle City/Tank Sprites")]
public class TankSpritesScriptableObject : ScriptableObject
{
    public Sprite[] Up;
    public Sprite[] Down;
    public Sprite[] Left;
    public Sprite[] Right;

    public Sprite GetSpriteOnDirection(Vector3 direction, int index)
    {
        if (direction == Vector3.up && Up.Length > index)
            return Up[index];
        if (direction == Vector3.down && Down.Length > index)
            return Down[index];
        if (direction == Vector3.right && Right.Length > index)
            return Right[index];
        if (direction == Vector3.left && Left.Length > index)
            return Left[index];
        if (direction == Vector3.zero && Up.Length > 0)
            return Up[0];

        return null;
    }
}


