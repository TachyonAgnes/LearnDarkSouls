using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LeftArmAnimrFix : MonoBehaviour
{
    private Animator anim;
    public Vector3 a;

   void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if(anim.GetBool("defense") == false)
        {
        Transform leftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        leftLowerArm.localEulerAngles += 0.75f * a;
        anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, Quaternion.Euler(leftLowerArm.localEulerAngles));
        }
    }
}
