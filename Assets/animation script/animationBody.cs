using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRawInput;
using UniRx;
using BackGroundGamepad;

public class animationBody : MonoBehaviour
{
    public AudioSource aud;
    private Transform ChildBody;
    private Animator BodyAnimator;

    private Transform ChildEye;
    private Animator EyeAnimator;

    private bool bodyR;
    private bool bodyL;
    private bool bodyPre;
    private bool bodyBack;
    private float size = 7f;

    private float mouse_x_before = 0;
    private float mouse_y_before = 0;

    private Transform Parent;
    private Animator ArmAtnimtor;

    public bool WorkInBackground;
    public bool InterceptMessages;

    private string trance = "key";
    private bool expression = false;

    private bool push = false;

    public BGPadListener GamePad;

    private void OnEnable()
    {
        RawKeyInput.Start(true);
        RawKeyInput.OnKeyUp += OnKeyUp;
        RawKeyInput.OnKeyDown += OnKeyDown;
    }

    private void OnDisable()
    {
        RawKeyInput.Stop();
        RawKeyInput.OnKeyUp -= OnKeyUp;
        RawKeyInput.OnKeyDown -= OnKeyDown;
    }

    private void OnKeyUp(RawKey key)
    {
        ArmAtnimtor.Play("armidle");
        if (expression == false)
        {
            push = false;
        }
        else
        {
            if (RawKey.S == key)
            {
                EyeAnimator.SetBool("shut", false);
            }
        }
    }

    // キーが押された時に呼び出される
    private void OnKeyDown(RawKey key)
    {
        ArmAtnimtor.Play("armsprint");
        if (RawKeyInput.IsKeyDown(RawKey.Home))
        {
            trance = "key";
        }

        if (RawKeyInput.IsKeyDown(RawKey.End))
        {
            trance = "stick";
        }

        if (RawKeyInput.IsKeyDown(RawKey.Delete))
        {
            trance = "mi";
        }

        if (RawKeyInput.IsKeyDown(RawKey.Insert))
        {
            if (expression)
            {
                expression = false;
            }
            else
            {
                expression = true;
            }
        }

        if (expression == true)
        {
            if (RawKeyInput.IsKeyDown(RawKey.S))
            {
                EyeAnimator.SetBool("shut", true);
            }
        }
    }

    void Start()
    {
        ChildBody = this.gameObject.transform.GetChild(3);
        BodyAnimator = ChildBody.GetComponent<Animator>();

        ChildEye = this.gameObject.transform.GetChild(4);
        EyeAnimator = ChildEye.GetComponent<Animator>();

        bodyR = false;
        bodyL = true;
        bodyPre = true;
        bodyBack = true;

        Parent = this.gameObject.transform.GetChild(2);
        ArmAtnimtor = Parent.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;

        float mouse_x_delta = mouse_x_before - x;
        float mouse_y_delta = mouse_y_before - y;

        mouse_x_before = x;
        mouse_y_before = y;

        float eyerandom = Random.Range(0, 200);
        bool eyeMove;

        if (eyerandom < 3)
        {
            eyeMove = true;
        }
        else
        {
            eyeMove = false;
        }

        if (trance == "mi")
        {
            moveArmtrance(mouse_x_delta, mouse_y_delta, eyeMove);
        }
        else if(trance == "key")
        {
            moveArm(mouse_x_delta, mouse_y_delta, eyeMove);
        }
        else if(trance == "stick")
        {
            xinput(eyeMove);
        }

        

       float random = Random.Range(0, 200);
 
        if (random < 10)
        {
            EyeAnimator.SetBool("wink", true);
        }
        else
        {
            EyeAnimator.SetBool("wink", false);
        }
    }

    private void moveArmtrance(float mouse_x_delta, float mouse_y_delta, bool eyeMove)
    {
        if (mouse_x_delta < -size && bodyR == false)
        {
            BodyAnimator.Play("bodyR");
            bodyR = true;
            bodyL = false;
            bodyPre = false;
            bodyBack = false;

            EyeAnimator.SetBool("R", eyeMove);
        }

        if (mouse_x_delta > size && bodyL == false)
        {
            BodyAnimator.Play("bodyL");
            bodyL = true;
            bodyR = false;
            bodyPre = false;
            bodyBack = false;

            EyeAnimator.SetBool("L", eyeMove);
        }

        if (mouse_y_delta > size && bodyPre == false)
        {
            BodyAnimator.Play("bodyPre");
            bodyPre = true;
            bodyR = false;
            bodyL = false;
            bodyBack = false;

            EyeAnimator.SetBool("pre", eyeMove);
        }

        if (mouse_y_delta < -size && bodyBack == false)
        {
            BodyAnimator.Play("bodyback");
            bodyBack = true;
            bodyR = false;
            bodyL = false;
            bodyPre = false;

            EyeAnimator.SetBool("back", eyeMove);
        }
    }


    private void moveArm(float mouse_x_delta, float mouse_y_delta, bool eyeMove)
    {
        if (mouse_x_delta < -size && bodyR == false)
        {
            BodyAnimator.Play("bodyR");
            bodyR = true;
            bodyL = false;
            bodyPre = false;
            bodyBack = false;

            EyeAnimator.SetBool("R", eyeMove);
        }

        if (mouse_x_delta > size && bodyL == false)
        {
            BodyAnimator.Play("bodyL");
            bodyL = true;
            bodyR = false;
            bodyPre = false;
            bodyBack = false;

            EyeAnimator.SetBool("L", eyeMove);
        }

        if (mouse_y_delta < -size && bodyPre == false)
        {
            BodyAnimator.Play("bodyPre");
            bodyPre = true;
            bodyR = false;
            bodyL = false;
            bodyBack = false;

            EyeAnimator.SetBool("pre", eyeMove);
        }

        if ( mouse_y_delta > size && bodyBack == false)
        {
            BodyAnimator.Play("bodyback");
            bodyBack = true;
            bodyR = false;
            bodyL = false;
            bodyPre = false;

            EyeAnimator.SetBool("back", eyeMove);
        }
    }

    private void xinput(bool eyeMove)
    {
        GamePad.OnTimeChanged.Subscribe(Key => {
            push = true;
        });

        if (push)
        {
            ArmAtnimtor.Play("armsprint");
            push = false;
        }
        else
        {
            ArmAtnimtor.Play("armidle");
        }

        Vector2 LeftStick = GamePad.GetLeftStick();
        float LeftStickLY = LeftStick.y;
        float LeftStickX = LeftStick.x;
        float stickSize = 20000f;
  
        if ( LeftStickX > stickSize && bodyR == false)
        {
            BodyAnimator.Play("bodyR");
            bodyR = true;
            bodyL = false;
            bodyPre = false;
            bodyBack = false;
            EyeAnimator.SetBool("R", eyeMove);
        }

        if ( LeftStickX < -stickSize && bodyL == false)
        {
            BodyAnimator.Play("bodyL");
            bodyL = true;
            bodyR = false;
            bodyPre = false;
            bodyBack = false;
            EyeAnimator.SetBool("L", eyeMove);

        }

        if ( LeftStickLY > stickSize && bodyPre == false)
        {
            BodyAnimator.Play("bodyPre");
            bodyPre = true;
            bodyR = false;
            bodyL = false;
            bodyBack = false;
            EyeAnimator.SetBool("pre", eyeMove);
        }

        if ( LeftStickLY < -stickSize && bodyBack == false)
        {
            BodyAnimator.Play("bodyback");
            bodyBack = true;
            bodyR = false;
            bodyL = false;
            bodyPre = false;
            EyeAnimator.SetBool("back", eyeMove);
        }
    }
}