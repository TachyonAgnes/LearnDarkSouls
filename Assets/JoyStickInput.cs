using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStickInput : IUserInput
{
    [Header("===== Joystick Settings =====")]
    public string axisX = "axisX";
    public string axisY = "axisY";
    public string axisJright = "axis4";
    public string axisJup = "axis5";
    public string axisLT = "axis9";
    public string btnA = "btn0";
    public string btnB = "btn1";
    public string btnX = "btn2";
    public string btnY = "btn3";
    public string btnRB = "btn5";
    public string btnLB = "btn4";
    public string btnJstick = "btn9";

    public MyButton buttonA = new MyButton();
    public MyButton buttonB = new MyButton();
    //public MyButton buttonX = new MyButton();
    //public MyButton buttonY = new MyButton();
    public MyButton buttonLB = new MyButton();
    public MyButton buttonRB = new MyButton();
    public MyButton buttonJstick = new MyButton();


    // Update is called once per frame
    void Update()
    {
        buttonA.Tick(Input.GetButton(btnA));
        buttonB.Tick(Input.GetButton(btnB));
        //buttonX.Tick(Input.GetButton(btnX));
        //buttonY.Tick(Input.GetButton(btnY));
        buttonLB.Tick(Input.GetButton(btnLB));
        buttonRB.Tick(Input.GetButton(btnRB));
        buttonJstick.Tick(Input.GetButton(btnJstick));

        Jup = -1 * Input.GetAxis(axisJup);
        Jright = Input.GetAxis(axisJright);

        targetDup = Input.GetAxis(axisY);
        targetDright = Input.GetAxis(axisX);

        if (inputEnabled == false)
        {
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

        isRun = (buttonA.IsPressing && !buttonA.IsDelaying) || buttonA.IsExtending;
        jump = buttonA.OnPressed && buttonA.IsExtending;
        roll = buttonA.IsDelaying && buttonA.OnReleased;
        defense = buttonLB.IsPressing;
        attack = buttonRB.OnPressed;
        lockon = buttonJstick.OnPressed;
    }

}
