using UnityEngine.InputSystem;
using System.Collections;
using Tweens;
using UnityEngine;
using Newtonsoft.Json.Serialization;


public enum FireMode
{
    SINGLE,
    AUTO,
    BURST
}


public class Firearm : Interactable
{
    public FireMode FireMode = FireMode.SINGLE;
    private int _roundPerMinute = 600;
    Vector3Tween recoilTween;
    Vector3 recoilReturnTween;
    private IEnumerator fireRoutine;

    private bool _canFire = true;

    public void Update()
    {
        if (GetFireInput() && _canFire)
        {
            fireRoutine = Fire();
            StartCoroutine(fireRoutine);
        }

        if (recoilTween == null)
        {
            RotationOffset = Vector3.Lerp(RotationOffset, Vector3.zero, 0.1f);
        }
    }


    private bool GetFireInput()
    {

        if (PrimaryInteractor == null) return false;

        bool inputTriggered = false;

        switch (FireMode)
        {
            case FireMode.SINGLE:
                PrimaryInteractor.Controller.HandActionMap.FindAction("TriggerPressed").performed += FireActionSingle;
                break;
            case FireMode.AUTO:
                inputTriggered = PrimaryInteractor.Controller.HandActionMap.FindAction("TriggerPressed").ReadValue<float>() != 0;
                break;
            case FireMode.BURST:
                PrimaryInteractor.Controller.HandActionMap.FindAction("TriggerPressed").performed += FireActionBurst;
                break;
        }

        return inputTriggered;
    }

    IEnumerator Fire()
    {
        _canFire = false;
        Recoil();
        yield return new WaitForSeconds(GetFireRate());
        _canFire = true;
    }


    private void FireActionSingle(InputAction.CallbackContext ctx)
    {
        fireRoutine = Fire();
        StartCoroutine(fireRoutine);
    }

    private void FireActionBurst(InputAction.CallbackContext ctx)
    {

        fireRoutine = Fire();
        StartCoroutine(fireRoutine);
    }

    private void Recoil()
    {
        recoilTween = new Vector3Tween
        {
            from = RotationOffset,
            to = RotationOffset - Vector3.left * -20f,
            duration = 0.1f,
            easeType = EaseType.QuadOut,
            onStart = (_) => { },
            onUpdate = (_, value) => RotationOffset = value,
            onFinally = (_) =>
            {
                recoilTween = null;
            }
        };

        gameObject.AddTween(recoilTween);
    }

    private void ReturnTween()
    {
        var tween = new Vector3Tween
        {
            from = RotationOffset,
            to = Vector3.zero,
            duration = 1f,
            easeType = EaseType.QuartInOut,
            onUpdate = (_, value) => RotationOffset = value,
        };

        gameObject.AddTween(tween);
    }

    public float GetFireRate()
    {
        return 60.0f / _roundPerMinute;
    }
}
