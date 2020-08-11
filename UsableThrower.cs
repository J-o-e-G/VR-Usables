using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableThrower : Usable
{

    // Adjustable public variables
    public GameObject prefab;
    public float throwForce = 250f;

    // Flag variable
    bool justPulledTrigger = false; 

    // On trigger press a <prefab> is instantiated and thrown forward
    public override int UseTrigger(float triggerPressure)
    {
        // If there is no prefab attached do nothing but put an alert in the console
        if(prefab == null)
        {
            Debug.Log("ALERT - Useble thrower needs a prefab attached in the inspector");
            return 1;
        }

        // Flag for toggle instead of continuous
        if (triggerPressure < 0.2f)
            justPulledTrigger = false;

        // This is the action to take on trigger press
        else if (!justPulledTrigger && triggerPressure > 0.25f)
        {
            // Create a <prefab> 
            GameObject newCube = Instantiate(prefab);
            
            // Start it 0.25 meters from barrel
            newCube.transform.position = transform.position + transform.TransformDirection(new Vector3(0, 0.25f, 0));

            // Add a force to its rigidbody in the direction of the barrel (if it has a rigidbody)
            Rigidbody rigidbody = newCube.GetComponent<Rigidbody>();
            if(rigidbody != null)
                rigidbody.AddForce(transform.TransformDirection(new Vector3(0, throwForce, 0)));            

            // Set this flag for semi-auto trigger use
            justPulledTrigger = true;
        }

        // Returns 0 so this DOES override the default trigger action, if it were 1 it would do both
        return 0;
    }

    // This device has no response to primary button input
    public override int UsePrimaryButton(bool puttonPressed)
    {
        // This will NOT override the default joystick action
        return 1;
    }

    // This device has no response to secondary button input
    public override int UseSecondaryButton(bool puttonPressed)
    {
        // This will NOT override the default joystick action        
        return 1;
    }

    // This device has no response to joystick input
    public override int UseJoystick(Vector2 joystickInput) {
        // This will NOT override the default joystick action
        return 1;
    }
}
