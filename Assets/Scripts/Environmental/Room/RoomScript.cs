﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class RoomScript : MonoBehaviour
{
    public List<GameObject> doors = new List<GameObject>();
    [SerializeField] private AudioSource audioTheme;

    private CameraController cameraController;
    private float desiredCameraHeight;

    private HashSet<GameObject> spawnedEnemies;
    [SerializeField] GameObject spawnerObject;
    private Spawner spawner;
    private int numEnemies;

    private bool playerWasHereBefore;


    /* Sets up the room.
     */
    private void Awake()
    {
        spawner = spawnerObject.GetComponent<Spawner>();
        playerWasHereBefore = false;        

        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        desiredCameraHeight = 19.7f;
    }


    /* Allows this Room to listen to each enemy it has spawned in order to
     * detect enemy death. This allows it to control when the room doors open.
     */
    private void SubscribeToEnemyDeath()
    {
        // Loop through each enemy
        foreach (GameObject enemyObject in spawnedEnemies)
        {
            Enemy enemy = enemyObject.gameObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.deathEvent += OnEnemyDied;
            }
        }
    }

    /* Called each time an enemy dies in the current room. Used to update the
     * enemy count and to handle the case when all enemies have been slain.
     */
    private void OnEnemyDied(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
        numEnemies--;

        // When all enemies in the room have been slain
        if (numEnemies == 0)
        {
            OpenAllDoors();
            StopThemeMusic();
            
        }
    }


    /* Opens all doors for the current room.
     */
    private void OpenAllDoors()
    {
        // If we have doors, open them
        if(doors.Count != 0)
        {
            foreach (GameObject door in doors)
            {
                if(door != null)
                {
                    DoorController doorController = door.GetComponent<DoorController>();
                    doorController.Open();
                }
            }
        }
        else
        {
            Debug.Log("No doors assigned");
        }        
    }


    /* Closes all doors for the current room.
     */
    private void CloseAllDoors()
    {
        // If we have doors, lock them
        if(doors.Count != 0)
        {
            foreach (GameObject door in doors)
            {
                if(door != null)
                {
                    DoorController doorController = door.GetComponent<DoorController>();
                    doorController.Close();
                }                
            }
        }
        else
        {
            Debug.Log("No doors assigned");
        }        
    }


    /* Plays the associated audio theme attached to the room object.
     */  
    private void PlayThemeMusic()
    {
        if (audioTheme != null && audioTheme.clip != null && !audioTheme.isPlaying)
        {
            audioTheme.Play();
        }
    }


    /* Stops the associated audio theme attached to the room object.
     */ 
    private void StopThemeMusic()
    {
        // Stop the theme music for the current room, if there is one
        if (audioTheme != null && audioTheme.clip != null)
        {
            audioTheme.Stop();
        }
    }


    /* Called when any other collision object enters this Room. Used to detect when the player
     * enters the room. If the player is entering the room for the first time and there are
     * enemies to spawn, then the room spawns them. Also regulates door locking/unlocking.
     */
    private void OnTriggerEnter(Collider other)
    {
        // We only care about the player entering the room
        if (other.gameObject.tag == "Player")
        {
            // Move the camera to this room's center
            cameraController.MoveTo(transform.position + new Vector3(0, desiredCameraHeight, 0));

            // If this is the first time the player is entering this room
            if(!playerWasHereBefore)
            {
                // And there are enemies to spawn
                if(spawner.Size() > 0)
                {
                    CloseAllDoors();

                    spawnedEnemies = spawner.SpawnEnemies();
                    numEnemies = spawnedEnemies.Count;
                    SubscribeToEnemyDeath();

                    PlayThemeMusic();
                }
                // This will only be the case for rooms that have no enemies
                else
                {
                    OpenAllDoors();
                }

                playerWasHereBefore = true;
            }
            // If instead the player has re-entered this room
            else
            {
                // Logically speaking, this seems a little paradoxical, but we need this check
                // because the room's trigger collider is slightly smaller than the room itself
                // Thus, it's possible that the player could "re-enter" a room from within the room
                if(numEnemies == 0)
                {
                    OpenAllDoors();
                }
            }
        }
    }
}
