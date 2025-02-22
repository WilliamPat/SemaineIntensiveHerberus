﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OpenDoor : MonoBehaviour 
{
    int playerID;
	public GameObject mainDoor;
	bool canTriggerCoroutine;
    AudioSource audioS;
    AudioClip activeDoor;

	void Start()
	{

        activeDoor = (AudioClip) Resources.Load("ActivationInterrupteur");
        playerID = GetComponent<Movement>().playerID;
        canTriggerCoroutine = true;
        mainDoor = GameObject.Find("Main_Door");
        audioS = GetComponent<AudioSource>();
        
		if (audioS == null)
		{
            audioS = gameObject.AddComponent<AudioSource>();
		}
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().gameObject.tag == "LeverDoor")
        {
            
        }
   }

    void OnTriggerStay(Collider other)
    {
        // Normal Doors
        if (other.GetComponent<Collider>().gameObject.tag == "LeverDoor")
        {
            
            if (XInput.instance.getTriggerRight(playerID) > 0.8f|| (Input.GetKey(KeyCode.Space)&&playerID==1))
			//if (Input.GetKey(KeyCode.Space))
			{	
                if (canTriggerCoroutine == true)
                {
                    Lever _Lever = other.GetComponent<Lever>();

                    if (_Lever.coroutineIsRunning == false)
                    {
						//StartCoroutine(FeedbackTimer());
                        _Lever.isIncreasing = true;
                        _Lever.StartCoroutine(other.GetComponent<Lever>().DoorState());
                        canTriggerCoroutine = false;
                        audioS.clip = activeDoor;
                        audioS.Play();
                    }
                }
            }
            else
            {
                if (canTriggerCoroutine == false)
                {
                    ResetVariables(other);
                }
            }
        }
            // Main door
            if (other.GetComponent<Collider>().gameObject.tag == "LeverMainDoor")
            {
                    MainDoorTrigger _RefToTrigger = other.GetComponent<MainDoorTrigger>();

                    if (_RefToTrigger.isActivated == false)
                    {
                        _RefToTrigger.isActivated = true;
						_RefToTrigger.ChangeState();
						
						if (mainDoor.GetComponent<MainDoor>())
						{
							mainDoor.GetComponent<MainDoor>().CheckDoorStatus();
							PoliceSpawnerManager.instance.eventSpawnOne();
						}

                        
                    }
                
            }
        }
       
    



	void ResetVariables(Collider lever)
	{
		// Lever
		if (lever.GetComponent<Lever>())
		{
			lever.GetComponent<Lever>().isIncreasing = false;
		}
        audioS.Stop();
        // Player
        canTriggerCoroutine = true;
	}
}