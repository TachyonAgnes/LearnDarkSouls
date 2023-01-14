using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour {
    public GameObject model;
    public CameraController camcon;
    public IUserInput pi;
    public float walkSpeed = 2.4f;
    public float runMultiplier = 2f;
    public float jumpVelocity = 4.4f;
    public float rollMuliplier = 1.2f;
    public float jabMultiplier = 20.0f;
    public float cAtkMuliplier = 2.0f;

    [Header("===== Friction Settings =====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    private Animator anim;
    private Rigidbody rigid;
    private Vector3 PlanarVec;
    private Vector3 thrustVec;
    private bool canAttack;
    private bool lockPlaner = false;
    private bool trackDirection = false;
    private CapsuleCollider col;
    //private float lerpTarget;
    private Vector3 deltaPos;

    [SerializeField]
    public bool leftIsShield = true;

    // Start is called before the first frame update
    void Awake() {
        IUserInput[] inputs = GetComponents<IUserInput>();
        foreach (var input in inputs) {
            if (input.enabled == true) {
                pi = input;
                break;
            }
        }
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update() {
        if (pi.lockon) {
            camcon.toggleLock();
        }
        if (camcon.lockState == false){ 
        anim.SetFloat("forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"), (pi.isRun ? 2.0f : 1.0f), 0.5f));
        anim.SetFloat("right", 0);
        }
        else {
            Vector3 localDvec = transform.InverseTransformVector(pi.Dvec);
            anim.SetFloat("forward", localDvec.z * (pi.isRun ? 2.0f : 1.0f));
            anim.SetFloat("right", localDvec.x * (pi.isRun ? 2.0f : 1.0f));
        }
        anim.SetFloat("velocity", rigid.velocity.magnitude);


       
        if (pi.roll|| rigid.velocity.magnitude > 8.0f) {
            anim.SetTrigger("roll");
            canAttack = false;
        }

        if (pi.jump) {
            anim.SetTrigger("jump");
            canAttack = false;
        }

        if ((pi.rb ||pi.lb) && (CheckState("ground") || CheckStateTag("attackR") || CheckStateTag("attackL")) && canAttack) {
            if (pi.rb) {
                anim.SetBool("R0L1",false);
                anim.SetTrigger("attack");
            }
            else if (pi.lb && !leftIsShield) {
                anim.SetBool("R0L1", true);
                anim.SetTrigger("attack");
            }
        }

        if((pi.rt||pi.lt)&&(CheckState("ground")||CheckState("attackR")||CheckState("attackL")&& canAttack)){
            if (pi.rt) {
                //do right heavy attack
            }
            else {
                if (!leftIsShield) {
                    //do left heavy attack
                }
                else {
                    anim.SetTrigger("counterBack");
                }
            }

        }


        if (anim.GetBool("isJab")) {
            if (pi.rb) {
                anim.SetTrigger("attack");
            }
        }

        if (leftIsShield) {
            if (CheckState("ground")||CheckState("blocked")) {
                anim.SetBool("defense", pi.defense);
                anim.SetLayerWeight(anim.GetLayerIndex("defense"), 1);
            }
            else {
                anim.SetBool("defense", false);
                anim.SetLayerWeight(anim.GetLayerIndex("defense"), 0);
            }
        }
        else {
            anim.SetLayerWeight(anim.GetLayerIndex("defense"), 0);
        }

        if (camcon.lockState == false) {
            if (pi.Dmag > 0.1f) {
                Vector3 targetForward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.5f);
                model.transform.forward = targetForward;
            }
            if (lockPlaner == false) {
                PlanarVec = pi.Dmag * model.transform.forward * walkSpeed * (pi.isRun ? runMultiplier : 1.0f);
            }    
        }
        else {
            if(trackDirection == false) {
                model.transform.forward = transform.forward;
            }
            else {
                model.transform.forward = PlanarVec.normalized;
            }
            if (lockPlaner == false) {
                PlanarVec = pi.Dvec * walkSpeed * (pi.isRun ? runMultiplier : 1.0f);
            }
        }

    }

    void FixedUpdate() {
        rigid.position += deltaPos;
        rigid.velocity = new Vector3(PlanarVec.x, rigid.velocity.y, PlanarVec.z) + thrustVec;
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }

    public bool CheckState(string stateName, string layerName = "Base Layer") {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }

    public bool CheckStateTag(string tagName, string layerName = "Base Layer") {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsTag(tagName);
    }

    /// <summary>
    /// Message processing block
    /// </summary>
    public void OnJumpEnter() {
        pi.inputEnabled = false;
        lockPlaner = true;
        thrustVec = new Vector3(0, jumpVelocity, 0);
        trackDirection = true;
    }
    public void OnRollEnter() {
        //thrustVec = new Vector3(0, rollVelocity, 0);
        pi.inputEnabled = false;
        lockPlaner = true;
        trackDirection = true;
    }

    public void OnJabEnter() {
        pi.inputEnabled = false;
        lockPlaner = true;
        trackDirection = false;
        anim.SetBool("isJab", true);
    }
    public void onJabCounterAttackEnter() {
        pi.inputEnabled = false;
        lockPlaner = true;
    }


    public void OnGroundEnter() {
        pi.inputEnabled = true;
        lockPlaner = false;
        canAttack = true;
        col.material = frictionOne;
        trackDirection = false;
    }
    public void OnGroundExit() {
        col.material = frictionZero;
    }
    public void OnFallEnter() {
        pi.inputEnabled = false;
        lockPlaner = true;
    }

    public void OnJabUpdate() {
        thrustVec = model.transform.forward * anim.GetFloat("jabVelocity") * jabMultiplier;
    }
    public void OnRollUpdate() {
        thrustVec = model.transform.forward * anim.GetFloat("rollVelocity") * rollMuliplier;
        col.material = frictionOne;
    }
    public void onJabCounterAttackUpdate() {
        thrustVec = model.transform.forward * anim.GetFloat("cAtkVelocity") * cAtkMuliplier;
        col.material = frictionOne;
    }


    public void IsGround() {
        anim.SetBool("isGround", true);
    }
    public void IsNotGround() {
        anim.SetBool("isGround", false);
    }

    public void OnAttack1hAEnter() {
        pi.inputEnabled = false;
    }

    public void OnAttack1hAUpdate() {
        thrustVec = model.transform.forward * anim.GetFloat("attack1hAVelocity");
    }

    public void OnUpdateRM(object _deltaPos) {
        if (CheckState("attack1hC")) {
            deltaPos += (0.8f * deltaPos + 0.2f * (Vector3)_deltaPos) * 1.0f;
        }
    }

    public void OnAttackExit(){
        model.SendMessage("WeaponDisable");
    }
    public void OnHitEnter() {
        pi.inputEnabled = false;
        PlanarVec = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }
    public void OnDieEnter() {
        pi.inputEnabled = false;
        PlanarVec = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }
    public void OnBlockedEnter() {
        PlanarVec = Vector3.zero;
    }

    public void OnStunnedEnter() {
        pi.inputEnabled = false;
        PlanarVec = Vector3.zero;
    }

    public void OnCounterBackEnter() {
        pi.inputEnabled = false;
        PlanarVec = Vector3.zero;
    }
    public void OnCounterBackExit() {
        model.SendMessage("CounterBackDisable");
    }

    public void IssueTrigger(string triggerName) {
        anim.SetTrigger(triggerName);
    }
}
