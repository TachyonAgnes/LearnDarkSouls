using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LeftArmAnimrFix : MonoBehaviour
{
    private Animator anim;
    private ActorController ac;
    public Vector3 a;

   void Awake()
    {
        anim = GetComponent<Animator>();
        ac = GetComponentInParent<ActorController>();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (ac.leftIsShield == true) {
            if (anim.GetBool("defense") == false) {
                Transform leftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                leftLowerArm.localEulerAngles += 0.75f * a;
                anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, Quaternion.Euler(leftLowerArm.localEulerAngles));
            }
        }
    }
}
