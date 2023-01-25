using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all the functions needed to support the eight axis system
/// </summary>

public static class EightAxisUtility
{
    public static Vector3[] direction = {
    new Vector3(1, 0, 1),           // N
    new Vector3(0, 0, 1),           // NW
    new Vector3(-0.4f, 0, 0.4f),    // W
    new Vector3(-1, 0, 0),          // SW
    new Vector3(-1, 0, -1),         // S
    new Vector3(0, 0, -1),          // SE
    new Vector3(0.4f, 0, -0.4f),    // E
    new Vector3(1, 0, 0),           // NE
    };

    // calculates the direction in 8 axis
    public static int EightAxisDirection(Vector2 _direction)
    {
        Vector2 norDirection = _direction.normalized;

        float axis = 360 / 8;
        float angle = Vector2.SignedAngle(Vector2.up, norDirection);

        if (angle < 0)
        {
            angle += 360;
        }

        return Mathf.FloorToInt(angle / axis);
    }

    // sets the correct directional sprite animations
    public static void SetSprite(Animator animator, string _animationNE, string _animationSE, Vector2 _direction)
    {
        if (_direction != Vector2.zero)
        {
            if (_direction.y > 0) // north
            {
                if (Mathf.Abs(_direction.x) > 0) // east or west
                {
                    animator.Play(_animationNE);
                }
                else
                {
                    animator.Play(_animationNE);
                }
            }
            else if (_direction.y < 0) // south
            {
                if (Mathf.Abs(_direction.x) > 0) // east or west
                {
                    animator.Play(_animationSE);
                }
                else
                {
                    animator.Play(_animationSE);
                }
            }
        }
    }

    // flips the sprite depending on direction
    public static void SpriteFlip(SpriteRenderer spriteRenderer, Vector2 _direction)
    {
        if (!spriteRenderer.flipX && _direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (spriteRenderer.flipX && _direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }
}
