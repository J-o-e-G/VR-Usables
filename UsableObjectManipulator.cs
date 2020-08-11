using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableObjectManipulator : Usable
{
    // Adjustable public variables
    [Tooltip("The effective range of the usable")]
    public float range = 200;
    [Tooltip("How quickly object is moved by joystick in hold mode")]
    public float distanceChangeSpeed = 10;
    [Tooltip("How quickly object is rotated by joystick in hold mode")]
    public float rotationSpeed = 10;

    // Flag variables
    bool justPressedTrigger, hold, justPressedPrimaryButton, justPressedSecondaryButton;

    // Internal links   
    GameObject objectOfInterest, displayObject;
    Transform previousParent;
    Rigidbody objectOfInterestRigidbody;
    LineRenderer lineRenderer;
    Vector3 offset;
    float holdDistance;

    private void Start()
    {
        // Get a link to the line renderer
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Allows player to get a reference to the object that this device is pointed at
    public override int UseTrigger(float triggerPressure)
    {
        // Reset the toggle flag if the player releases the trigger
        if (triggerPressure < 0.1f)
            justPressedTrigger = false;

        // If there is any pressure on the trigger turn on the line renderer for easy aiming
        if (triggerPressure > 0.01f)
            LineOn();
        else
            LineOff();

        // If player presses the trigger get the object that they are pointed at
        if (!justPressedTrigger && triggerPressure > 0.4f)
        {
            // Send out a raycast and store what is hit
            RaycastHit hit;
            if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, range))
            {
                // Get a reference to the object that was hit
                objectOfInterest = hit.collider.gameObject;

                // If there is an object get a few references form it
                if (objectOfInterest != null)
                {
                    // Get a reference to the objects ridgidbody if it has one
                    objectOfInterestRigidbody = objectOfInterest.GetComponent<Rigidbody>();

                    // Keep track of the objects parent so duplicates can be places into the heirarchy
                    previousParent = objectOfInterest.transform.parent;
                    
                    // Adjust the hold distance
                    holdDistance = (objectOfInterest.transform.position - transform.position).magnitude;
                }
            }

            // Flag for toggle 
            justPressedTrigger = true;
        }

        // This will override the normal trigger action
        return 0;
    }
   
    // Primary button allows the player to move the object of interest
    public override int UsePrimaryButton(bool buttonPressed)
    {
        // Reset toggle flag
        if (!buttonPressed)
            justPressedPrimaryButton = false;

        // If button is pressed toggle if the usable is moving the object of interest
        if (!justPressedPrimaryButton && buttonPressed)
        {
            // Toggle the hold variable
            hold = !hold;

            // Set the toggle flag to prevent rapid reversal
            justPressedPrimaryButton = true;

            // Get the objects position relative to the usable
            if (objectOfInterest != null)
            {
                // The distance between the usable and the object of interest
                holdDistance = (objectOfInterest.transform.position - transform.position).magnitude;

                // Hold position is the distance projected along usable's z axis
                Vector3 holdPosition = CalculateHoldPosition();

                // Get an offset vector so the object only moves when the plaer moves the usable
                offset = objectOfInterest.transform.position - holdPosition;
            }
        }

        // Move the object with the usable while the button is being held
        HoldAtPosition();

        // This will override the normal primary button action
        return 0;
    }
    
    // The secondary button duplicates the object of interest
    public override int UseSecondaryButton(bool secondaryButtonPressed)
    {
        if (!secondaryButtonPressed)
            justPressedSecondaryButton = false;

        // Duplicate the last object that the script found a reference to
        if (!justPressedSecondaryButton && secondaryButtonPressed)
        {
            // Makea duplicate of the object of interest and place it
            Duplicate();

            // Set toggle flag
            justPressedSecondaryButton = true;
        }

        // This will NOT override the normal secondary button action
        return 1;
    }
    
    // Joystick moves and rotates object only when there is an object being actively moved
    public override int UseJoystick(Vector2 joystickInput)
    {
        // If player is adjusting an objects position
        if (hold && objectOfInterest != null)
        {
            // Move the distance (object's distance from player) based on y input
            holdDistance += joystickInput.y * distanceChangeSpeed * Time.deltaTime;

            // Rotate object based on joystick x input
            objectOfInterest.transform.Rotate(0,joystickInput.x,0);

            // This will override the normal joystick action
            return 0;
        }

        // This will NOT override the normal joystick action
        return 1;                
    }
    
    // Helper functions
    void HoldAtPosition()
    {
        // If theres no object or the usable is not being held reset the hold flag
        if (objectOfInterest == null || beingHeld == false)       
            hold = false;

        // Hold position is the distance projected along usable's z axis
        Vector3 holdPosition = CalculateHoldPosition();

        // Move the object of interest based on the usable 
        if (hold)
        {
            // Move the object of interest to the hold position + the offset
            objectOfInterest.transform.position = holdPosition + offset;

            // Reset the objects velocity to 0 so it does not build up
            if (objectOfInterestRigidbody != null)
                objectOfInterestRigidbody.velocity = Vector3.zero;
        }
    }    
    void Duplicate()
    {
        // If there is an object instantiate a new copy of it
        if(objectOfInterest != null)
        {
            // Instantiation
            GameObject newObject = Instantiate(objectOfInterest);

            // Get a reference to the duplicated objects ridgidbody if it has one
            objectOfInterestRigidbody = objectOfInterest.GetComponent<Rigidbody>();

            // Calculate the hold position based on the device rotation and <holdDistance>
            Vector3 holdPosition = CalculateHoldPosition();

            // Set its position
            newObject.transform.position = holdPosition;

            // Place it into the heirarchy
            if (previousParent != null)
                newObject.transform.parent = previousParent;

            // Set it as the new object of interest
            objectOfInterest = newObject;
        }
    }
    Vector3 CalculateHoldPosition()
    {
        // The hold position is projected <holdDistance> units along the transform direction
        Vector3 holdPosition = transform.position + transform.TransformDirection(new Vector3(0, 0, holdDistance));

        // Return the found value
        return holdPosition;
    }
    Vector3 CalculateHoldPosition(float alternateDistance)
    {
        // The hold position is projected <holdDistance> units along the transform direction
        Vector3 holdPosition = transform.position + transform.TransformDirection(new Vector3(0, 0, alternateDistance));

        // Return the found value
        return holdPosition;
    }
    void LineOn()
    {
        // Avoid errors if there is no line renderer
        if (lineRenderer == null)
            return;

        // Turn it on
        lineRenderer.enabled = true;

        // Set its positions
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, CalculateHoldPosition(range));

    }
    void LineOff()
    {
        // Avoid errors if there is no line renderer
        if (lineRenderer == null)
            return;

        // Turn off the line renderer
        lineRenderer.enabled = false;
    }    

}
