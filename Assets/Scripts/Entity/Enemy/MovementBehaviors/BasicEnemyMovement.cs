﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    Basic enemy movement that moves towards the player.
 */
[RequireComponent(typeof(EnemyMovement))]
public class BasicEnemyMovement : MonoBehaviour, IOnDeathController
{
    private EnemyMovement em;
    private Transform playerTransform;

    public void OnDeath()
    {
        enabled = false;
        em.Stop();
    }

    void Awake()
    {
        em = GetComponent<EnemyMovement>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        em.MoveTo(playerTransform.position);
    }
}