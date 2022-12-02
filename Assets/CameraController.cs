using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    private IUserInput pi;
    public float horizontalSpeed = 80.0f;
    public float verticalSpeed = 80.0f;
    public float cameraDampValue = 0.5f;
    public Image lockDot;
    public bool lockState;


    private GameObject playerHandle;
    private GameObject cameraHandle;
    private float tempEulerX;
    private GameObject model;
    private GameObject camera;

    private Vector3 cameraDampVelocity;
    [SerializeField]
    private LockTarget lockTarget;


    // Start is called before the first frame update
    void Start() {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempEulerX = 20.0f;
        ActorController ac = playerHandle.GetComponent<ActorController>();
        model = ac.model;
        pi = ac.pi;
        camera = Camera.main.gameObject;
        lockDot.enabled = false;
        lockState = false;
    }

    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (lockTarget == null) {
            Vector3 tempModelEuler = model.transform.eulerAngles;
            playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);
            tempEulerX -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -40, 30);
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);

            model.transform.eulerAngles = tempModelEuler;
        }
        else {
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;
            tempForward.y = 0;
            playerHandle.transform.forward = tempForward;
            cameraHandle.transform.LookAt(lockTarget.obj.transform.position);
        }


        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, transform.position, ref cameraDampVelocity, cameraDampValue);
        //camera.transform.eulerAngles = transform.eulerAngles;
        camera.transform.LookAt(cameraHandle.transform);
    }

    void Update() {
        if (lockTarget != null) {
            lockDot.transform.position = Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position + new Vector3(0, lockTarget.halfHeight, 0));
            if(Vector3.Distance(model.transform.position, lockTarget.obj.transform.position)> 10.0f) {
                lockTarget = null;
                lockDot.enabled = false;
                lockState = false;
            }
        }
    }
    public void toggleLock() {
        //try to lock
        //Vector3 modelOrigin1 = model.transform.position;
        //Vector3 modelOrigin2 = modelOrigin1 + new Vector3(0, 1, 0);
        //Vector3 boxCenter = modelOrigin2 + model.transform.forward * 5.0f;
        //Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5f), model.transform.rotation, LayerMask.GetMask("Enemy"));

        //mine
        Vector3 modelOrigin1 = camera.transform.position;
        Vector3 modelOrigin2 = modelOrigin1 + new Vector3(0, 1, 0);
        Vector3 boxCenter = modelOrigin2 + camera.transform.forward * 5.0f;
        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.7f, 0.7f, 5f), camera.transform.rotation, LayerMask.GetMask("Enemy"));

        if (cols.Length == 0) {
            lockTarget = null;
            lockDot.enabled = false;
            lockState = false;

        }
        else {
            foreach (var col in cols) {
                if (lockTarget != null && lockTarget.obj == col.gameObject) {
                    lockTarget = null;
                    lockDot.enabled = false;
                    lockState = false;
                    break;
                }
                lockDot.enabled = true;
                lockTarget = new LockTarget(col.gameObject, col.bounds.extents.y);
                lockState = true;
                break;
            }
        }
    }

    private class LockTarget {
        public GameObject obj;
        public float halfHeight;

        public LockTarget(GameObject _obj,float _halfHeight){
            obj = _obj;
            halfHeight = _halfHeight;
        }
    }

}
