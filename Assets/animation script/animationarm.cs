using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRawInput;

public class animationarm : MonoBehaviour
{
    private Transform Parent;
    private Animator ArmAtnimtor;

    public bool WorkInBackground;
    public bool InterceptMessages;

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
    }

    // キーが押された時に呼び出される
    private void OnKeyDown(RawKey key)
    {
        ArmAtnimtor.Play("armsprint");
    }


    void Start()
    {
        Parent = this.gameObject.transform.GetChild(2);
        ArmAtnimtor = Parent.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
