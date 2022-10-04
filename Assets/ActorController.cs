using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public PlayerInput pi;
    public float walkSpeed = 1.4f;
    public float runMultiplier = 2.7f;
    public float jumpVelocity = 2.5f;
    public float rollVelocity = 3.0f;
    public float jabMultiplier = 3.0f;

    [SerializeField]
    private Animator anim;
    private Rigidbody rigid;
    private Vector3 PlanarVec;
    private Vector3 thrustVec;

    private bool lockPlaner = false;

    // Start is called before the first frame update
    void Awake()
    {
        pi = GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
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
        }
        
        if(pi.Dmag > 0.1f)
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
        rigid.velocity = new Vector3(PlanarVec.x, rigid.velocity.y, PlanarVec.z) + thrustVec;
        thrustVec = Vector3.zero;
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
        thrustVec = new Vector3(0, rollVelocity, 0);
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

    public void IsGround()
    {
        anim.SetBool("isGround", true);
    }
    public void IsNotGround()
    {
        anim.SetBool("isGround", false);
    }
}
