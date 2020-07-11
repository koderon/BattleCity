using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(InputSystem))]
public sealed class InputSystem : UpdateSystem
{
    private Filter filter;

    public override void OnAwake()
    {
        filter = World.Filter.With<InputComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        var inputs = filter.Select<InputComponent>();

        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        
        var dir = new Vector2(x, y);

        var direction = Vector2.zero;
        if (dir.sqrMagnitude > 0.01f)
        {
            if (Mathf.Abs(x) > Mathf.Abs(y))
                direction = x > 0f ? Vector2.right : Vector2.left;
            else
                direction = y > 0f ? Vector2.up : Vector2.down;
        }

        /*
        if (direction.magnitude >= 0.5f)
            Debug.Log(direction + " mag := " + direction.magnitude);*/

        for (int i = 0; i < filter.Length; i++) 
        {
            ref var input = ref inputs.GetComponent(i);

            input.Direction = direction;
        }
    }
}