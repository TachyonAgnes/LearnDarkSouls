using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IUserInput : MonoBehaviour
{

    [Header("===== Output signals =====")]
    public float Dup;
    public float Dright;
    public float Dmag;
    public Vector3 Dvec;

    public float Jup;
    public float Jright;

    // 1. pressing signal
    public bool isRun;
    // 2. trigger once signal
    public bool jump;
    protected bool lastJump;
    public bool attack;
    protected bool lastAttack;
    // 3. double trigger

    [Header("===== Others =====")]
    public bool inputEnabled = true;

    protected float targetDup;
    protected float targetDright;
    protected float velocityDup;
    protected float velocityDright;

    protected Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - input.y * input.y / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - input.x * input.x / 2.0f);

        return output;
    }
}
