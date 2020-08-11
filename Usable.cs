using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Input Script can have a Useble variable that is a place holder for many useble objects by using an abstract class
public abstract class Usable : MonoBehaviour
{
    // This flag ensures only one controller at a time is holding the object (used in controller scripts)
    public bool beingHeld = false;

    // These methods will be called from Input scripts on different kinds of usable objects
    public abstract int UseTrigger(float triggerPressure);
    public abstract int UsePrimaryButton(bool buttonPressed);
    public abstract int UseSecondaryButton(bool buttonPressed);
    public abstract int UseJoystick(Vector2 joystickInput);
    
    // Returning 0 overrides the default trigger action, if it were 1 it would do both
}
