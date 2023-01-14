using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : IUserInput {
    // Variable
    [Header("===== key settings =====")]
    public string keyUp = "w";
    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";
    public string keyLock = "q";

    public string keyA;
    public string keyB;
    //public string keyX;
    //public string keyY;
    public string keyLB;
    public string keyRB;

    public MyButton myKeyA = new MyButton();
    public MyButton myKeyB = new MyButton();
    //public MyButton myKeyX = new MyButton();
    //public MyButton myKeyY = new MyButton();
    public MyButton myKeyLB = new MyButton();
    public MyButton myKeyRB = new MyButton();

    //view
    public string keyJRight;
    public string keyJLeft;
    public string keyJUp;
    public string keyJDown;

    [Header("==== Mouse settings ====")]
    public bool mouseEnable = false;
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;

    // Update is called once per frame
    void Update() {
        myKeyA.Tick(Input.GetKey(keyA));
        myKeyB.Tick(Input.GetKey(keyB));
        //myKeyX.Tick(Input.GetKey(keyX));
        //myKeyY.Tick(Input.GetKey(keyY));
        myKeyLB.Tick(Input.GetKey(keyLB));
        myKeyRB.Tick(Input.GetKey(keyRB));


        Jup = (Input.GetKey(keyJUp) ? 1.0f : 0) - (Input.GetKey(keyJDown) ? 1.0f : 0)
               + Input.GetAxis("Mouse Y") * mouseSensitivityY * 1.0f;
        Jright = (Input.GetKey(keyJRight) ? 1.0f : 0) - (Input.GetKey(keyJLeft) ? 1.0f : 0)
               + Input.GetAxis("Mouse X") * mouseSensitivityX * 1.0f;

        targetDup = (Input.GetKey(keyUp) ? 1.0f : 0) - (Input.GetKey(keyDown) ? 1.0f : 0);
        targetDright = (Input.GetKey(keyRight) ? 1.0f : 0) - (Input.GetKey(keyLeft) ? 1.0f : 0);


        if (inputEnabled == false) {
            targetDup = 0;
            targetDright = 0;
        }

        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;

        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));
        Dvec = Dright2 * transform.right + Dup2 * transform.forward;

        isRun = (myKeyB.IsPressing && !myKeyB.IsDelaying) || myKeyB.IsExtending;
        jump = myKeyA.OnPressed && (myKeyB.IsPressing && !myKeyB.IsDelaying);
        roll = myKeyA.IsDelaying && myKeyA.OnReleased;
        defense = myKeyLB.IsPressing;
        rb = myKeyRB.OnPressed;
        lockon = Input.GetKey(keyLock);
    }
}
