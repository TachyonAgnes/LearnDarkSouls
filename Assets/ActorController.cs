using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public IUserInput pi;
    public float walkSpeed = 1.4f;
    public float runMultiplier = 2.7f;
    public float jumpVelocity = 2.5f;
    public float rollMuliplier = 3.0f;
    public float jabMultiplier = 3.0f;

    [Header("===== Friction Settings =====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    private Animator anim;
    private Rigidbody rigid;
    private Vector3 PlanarVec;
    private Vector3 thrustVec;
    private bool canAttack;
    private bool lockPlaner = false;
    private CapsuleCollider col;
    private float lerpTarget;
    private Vector3 deltaPos;

    // Start is called before the first frame update
    void Awake()
    {
        IUserInput[] inputs = GetComponents<IUserInput>();
        foreach(var input in inputs)
        {
            if(input.enabled == true)
            {
                pi = input;
                break;
            }
        }
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"), (pi.isRun ? 2.0f : 1.0f), 0.5f));
        if (rigid.velocity.magnitude > 0)
        {
            anim.SetTrigger("roll");
        }
        if (pi.jump)
        {
            anim.SetTrigger ("jump");
            canAttack = false;
        }

        if (pi.attack && CheckState("ground") && canAttack)
        {
            anim.SetTrigger("attack");
        }

        if (pi.Dmag > 0.1f)
        {
            Vector3 targetForward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.5f);
            model.transform.forward = targetForward;
        }
        if(lockPlaner == false)
        {
            PlanarVec = pi.Dmag * model.transform.forward * walkSpeed * (pi.isRun ? runMultiplier : 1.0f);
        }
    }

    void FixedUpdate()
    {
        rigid.position += deltaPos;
        rigid.velocity = new Vector3(PlanarVec.x, rigid.velocity.y, PlanarVec.z) + thrustVec;
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }

    private bool CheckState(string stateName, string layerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }

    /// <summary>
    /// Message processing block
    /// </summary>
    public void OnJumpEnter()
    {
        pi.inputEnabled = false;
        lockPlaner = true;
        thrustVec = new Vector3(0, jumpVelocity, 0);
    }
    public void OnRollEnter()
    {
        //thrustVec = new Vector3(0, rollVelocity, 0);
        pi.inputEnabled = false;
        lockPlaner = true;
    }

    public void OnJabEnter()
    {
        
        pi.inputEnabled = false;
        lockPlaner = true;
    }

    public void OnGroundEnter()
    {
        pi.inputEnabled = true;
        lockPlaner = false;
        canAttack = true;
        col.material = frictionOne;
    }
    public void OnGroundExit()
    {
        col.material = frictionZero;
    }
    public void OnFallEnter()
    {
        pi.inputEnabled = false;
        lockPlaner = true;
    }

    public void OnJabUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("jabVelocity") * jabMultiplier;
    }
    public void OnRollUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("rollVelocity") * rollMuliplier;
    }

    public void IsGround()
    {
        anim.SetBool("isGround", true);
    }
    public void IsNotGround()
    {
        anim.SetBool("isGround", false);
    }

    public void OnAttack1hAEnter()
    {
        pi.inputEnabled = false;
        lerpTarget = 1.0f;
    }
    public void OnAttack1hAUpdate() 
    {
        thrustVec = model.transform.forward * anim.GetFloat("attack1hAVelocity");
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, 0.4f));
    }
    public void OnAttackIdleEnter()
    {
        pi.inputEnabled = true;
        lerpTarget = 0f;
    }
    public void OnAttackIdleUpdate()
    {
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, 0.4f));
    }

    public void OnUpdateRM(object _deltaPos)
    {
        if (CheckState("attack1hC","attack"))
        {
            deltaPos += (Vector3)_deltaPos;
        }
        
    }
}
