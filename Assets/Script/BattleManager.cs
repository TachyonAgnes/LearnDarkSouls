using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class BattleManager : IActorManagerInterface
{
    private CapsuleCollider defCol;
    private void Start() {
        //auto generate and define capsule collider
        defCol = GetComponent<CapsuleCollider>();
        defCol.center = Vector3.up;
        defCol.height = 2.0f;
        defCol.radius = 0.3f;
        defCol.isTrigger = true;
    }
    private void OnTriggerEnter(Collider col) {
        WeaponController targetWc = col.GetComponentInParent<WeaponController>();
        GameObject attacker = targetWc.wm.am.ac.model.gameObject;
        GameObject receiver = am.ac.model.gameObject;

        Vector3 attackingDir = receiver.transform.position - attacker.transform.position;
        Vector3 counterDir = attacker.transform.position - receiver.transform.position;

        float attackingAngle1 = Vector3.Angle(attacker.transform.forward, attackingDir);
        float counterAngle1 = Vector3.Angle(receiver.transform.forward, counterDir);
        float counterAngle2 = Vector3.Angle(attacker.transform.forward, receiver.transform.forward); //should be closed to 180 degree

        bool attackValid = attackingAngle1 < 80;
        bool counterValid = (counterAngle1 < 120 && Mathf.Abs(counterAngle2 - 180) < 120);
        if (col.tag == "Weapon") {
            am.TryDoDamage(targetWc, attackValid, counterValid);
        }
    }
}
