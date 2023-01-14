using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public ActorController ac;

    [Header("=== Auto Find or Generate If Null ===")]
    public BattleManager bm;
    public WeaponManager wm;
    public StateManager sm;
    // Start is called before the first frame update
    void Awake()
    {
        ac = GetComponent<ActorController>();
        // build bm
        GameObject model = ac.model;
        GameObject sensor = transform.Find("sensor").gameObject;
        bm = Bind<BattleManager>(sensor);
        wm = Bind<WeaponManager>(model);
        sm = Bind<StateManager>(gameObject);

    }
    private T Bind<T>(GameObject gObj) where T: IActorManagerInterface{
        T tempInstance;
        tempInstance = gObj.GetComponent<T>();
        if (tempInstance == null) {
            tempInstance = gObj.AddComponent<T>();
        }
        tempInstance.am = this;
        return tempInstance;
    }

    public void SetIsCounterBack(bool value) {
        sm.isCounterBackEnable = value;
    }

    public void TryDoDamage(WeaponController targetWc, bool attackValid, bool counterValid) {
        if (sm.isCounterBackSuccess) {
            if (counterValid) {
                targetWc.wm.am.Stunned();
            }
        }
        else if(sm.isCounterBackFailure) {
            if (attackValid) {
                HitOrDie(false);
            }
        }
        else if (sm.isImmortal) {
            //do Nothing
        }else if (sm.isDefense) {
            Blocked();
        }
        else {
            if (attackValid) {
                HitOrDie(true); 
            }
        }
    }

    public void HitOrDie( bool doHitAnimation) {
        if (sm.HP <= 0) {
            //Already dead;
        }
        else {
            sm.AddHp(-5f);
            if (sm.HP > 0) {
                if (doHitAnimation) {
                    Hit();
                }
                //do some VFX
            }
            else {
                Die();
            }
        }
    }

    public void Stunned() {
        ac.IssueTrigger("stunned");
    }

    public void Blocked() {
        ac.IssueTrigger("blocked");
    }
    public void Hit() {
        ac.IssueTrigger("hit");
    }

    public void Die() {
        ac.IssueTrigger("die");
        ac.pi.inputEnabled = false;
        if(ac.camcon.lockState == true) { 
            ac.camcon.toggleLock();
        }
       //ac.camcon.enabled = false;
    }
}