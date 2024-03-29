﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FieldOfView : MonoBehaviour, IOnDeathController
{
    [Tooltip("The maximum distance at which targets can be sighted")]
    public float viewRadius;

    [Tooltip("The total field of view angle")]
    [Range(0, 360)] public float viewAngle;

    [Tooltip("The layer on which the player resides")]
    public LayerMask playerMask;

    [Tooltip("The layer on which the player's projectiles reside")]
    public LayerMask playerProjectileMask;

    [Tooltip("The layer on which the obstacles reside")]
    public LayerMask obstacleMask;


    /* Returns true if the enemy can see any objects belonging to the given layer,
     * and false otherwise.
     */ 
    private bool CanSeeSomethingOn(LayerMask layer)
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, layer);

        foreach (Collider targetCollider in targetsInViewRadius)
        {
            Transform player = targetCollider.gameObject.transform;

            Vector3 targetDirection = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, targetDirection);

            // Target object is within the FOV angle
            if (angleToPlayer < viewAngle / 2.0f)
            {
                float distanceToTarget = Vector3.Distance(transform.position, player.position);

                // If there is no obstacle in between us and the target object, then we've found it
                if (!Physics.Raycast(transform.position, targetDirection, distanceToTarget, obstacleMask))
                {
                    return true;
                }
            }
        }

        return false;
    }



    /* Returns true if the player is within the field of view cone, and false otherwise.
     */
    public bool PlayerWithinView()
    {
        return CanSeeSomethingOn(playerMask);
    }


    /* Returns true if the player's projectiles have been spotted, and false otherwise.
     */ 
    public bool PlayerProjectileWithinView()
    {
        return CanSeeSomethingOn(playerProjectileMask);
    }


    /* Returns a normal vector representing the direction of the given angle.
     */
    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        // If the given angle is relative to the player, transform it to global as seen from above (looking opposite the y axis)
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        // Note: cosine and sine are effectively swapped in Unity
        // Moreover, keep in mind that we're in the xz plane
        return new Vector3(Mathf.Sin(angleInRadians), 0, Mathf.Cos(angleInRadians)).normalized;
    }


    /* Called when the associated enemy dies.
     */
    public void OnDeath()
    {
        enabled = false;
    }
}
