using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : IActorManagerInterface
{
    private Collider weaponColL;
    private Collider weaponColR;

    public GameObject whL;
    public GameObject whR;

    public WeaponController wcL;
    public WeaponController wcR;

    private void Start() {
        whL = transform.DeepFind("weaponHandleL").gameObject;
        whR = transform.DeepFind("weaponHandleR").gameObject;

        wcL = BindWeaponController(whL);
        wcR = BindWeaponController(whR);

        weaponColL = whL.GetComponentInChildren<Collider>();
        weaponColR = whR.GetComponentInChildren<Collider>();

        weaponColR.enabled = false;
        weaponColL.enabled = false;
    }

    public WeaponController BindWeaponController(GameObject target0bj) {
        WeaponController tempWc;
        tempWc = target0bj.GetComponent<WeaponController>();
        if (tempWc == null) {
            tempWc = target0bj.AddComponent<WeaponController>();
        }
        tempWc.wm = this;
        return tempWc;
    }

    public void WeaponEnable() {
        if (am.ac.CheckStateTag("attackL")) {
            weaponColL.enabled = true;
        }
        else {
            weaponColR.enabled = true;
        }
    }

    public void WeaponDisable() {
        weaponColL.enabled = false;
        weaponColR.enabled = false;
    }

    public void CounterBackEnable() {
        am.SetIsCounterBack(true);
    }

    public void CounterBackDisable() {
        am.SetIsCounterBack(false); 
    }
}
