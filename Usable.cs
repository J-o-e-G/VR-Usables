using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abstract class gives contoller input script a way to interact with many usable objects
public abstract class Usable : MonoBehaviour
{
    // This flag ensures only one controller at a time is holding the object
    public bool beingHeld = false;

    // These methods will be called from controller input scripts, returning 0 overrides the default action
    public abstract int UseTrigger(float triggerPressure);
    public abstract int UsePrimaryButton(bool buttonPressed);
    public abstract int UseSecondaryButton(bool buttonPressed);
    public abstract int UseJoystick(Vector2 joystickInput);      
}
