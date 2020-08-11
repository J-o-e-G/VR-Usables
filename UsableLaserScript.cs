using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableLaserScript : Usable
{
    // Adjustable public variables
    public float laserRange = 200;

    // Internal references
    LineRenderer lineRenderer;
    ParticleSystem sparks;
    LayerMask notPlayer;

    private void Start()
    {
        // Get a reference to the line renderer and turn it off
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
            lineRenderer.enabled = false;

        // Get a reference to the sparks prefab and turn it off
        sparks = GetComponentInChildren<ParticleSystem>();
        if (sparks != null)
            sparks.gameObject.SetActive(false);

        // Define a layer mask, binary opposite =~ means everything but these
        notPlayer =~ LayerMask.GetMask("Player", "Usable");
    }

    // The trigger controlls the laser activity and width for this device
    public override int UseTrigger(float triggerPressure)
    {
        // Turn off beam when trigger is released
        if (triggerPressure < 0.02f)
            BeamOff();
        // Turn on when its pressed
        else
            BeamOn(triggerPressure);       

        // Returns 0 so this overrides the default trigger action, if it were 1 it would do this and the default action
        return 0;
    }

    // This device has no response to primary button input
    public override int UsePrimaryButton(bool puttonPressed)
    {
        // This will NOT override the default action        
        return 1;
    }

    // This device has no response to secondary button input
    public override int UseSecondaryButton(bool puttonPressed)
    {
        // This will NOT override the default action        
        return 1;
    }

    // This device has no response to joystick input
    public override int UseJoystick(Vector2 joystickInput)
    {
        // This will NOT override the default action        
        return 1;
    }

    // Helper functions
    void BeamOn(float intensity)
    {
        // Turn on line renderer, set starting point and intensity
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.startWidth = 0.05f * intensity;
        lineRenderer.endWidth = 0.05f * intensity;

        // Send out a raycast and set second point based on output
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, laserRange, notPlayer))
        {
            lineRenderer.SetPosition(1, hit.point);

            // Show sparks
            if (sparks != null)
            {
                sparks.transform.position = hit.point;
                sparks.gameObject.SetActive(true);
                sparks.transform.localScale = new Vector3(1, 1, 1) * 2 * intensity;
            }


            // Look for a baloon to pop
            Baloon baloonToPop = hit.collider.GetComponent<Baloon>();
            if (baloonToPop != null)
            {
                baloonToPop.inheritVelocity = true;
                baloonToPop.Pop();
            }
        }
        // If it did not hit anything set the second line renderer point to 200 meters out and disable sparks
        else
        {
            // Set second point to 200 meters out
            if(lineRenderer != null)
                lineRenderer.SetPosition(1, transform.position + transform.TransformDirection(new Vector3(0, 0, laserRange)));

            // Disable sparks
            if (sparks != null)
                sparks.gameObject.SetActive(false);
        }
    }
    void BeamOff()
    {
        // Turn off the line renderer
        if (lineRenderer != null)
            lineRenderer.enabled = false;

        // Disable sparks
        if (sparks != null)
            sparks.gameObject.SetActive(false);
    }
}

