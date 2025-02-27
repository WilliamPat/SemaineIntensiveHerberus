﻿using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {
    protected Transform policeStation;
    protected float updateRate = 0.4f;
    public Transform hand;
    protected GameObject currentItem;
    protected string targetTag = "Trophy";
    protected Transform trophy;
    protected Transform currentTarget;
    public float stunTime = 1;
    protected NavMeshAgent agent;
    public float speedTime, speedMulti;
    public float waitingTime = 1;
    public Animation anim;
    public float dropItemForce=10;

    protected virtual void Start () {
        currentItem = null;
        policeStation = GameObject.Find("PoliceStation").transform;
        trophy = GameObject.FindGameObjectWithTag(targetTag).transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animation>();
        if(anim)
        anim.Play();
        InvokeRepeating("personnalBehavior", updateRate, updateRate);

	}
	
	//void Update () {
    //  agent.SetDestination(target.position);
	//}

    protected virtual void personnalBehavior()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        if (currentTarget&&agent.enabled)
            agent.SetDestination(currentTarget.position);
    }

    protected virtual void playerHit(GameObject playerHit)
    {
        StartCoroutine(speedBuff());
        personnalBehavior();
        playerHit.GetComponentInParent<PlayerStun>().startStun(stunTime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player"&& !GetComponent<EnemyStun>().isStun && currentItem==null)
        {
            GameObject go = col.collider.GetComponentInParent<PlayerHand>().currentItem;
            if (go && go.tag == targetTag)
            {
                currentItem = go;
                go = null;
                // Take Trophy & stun
                takeTrophy();
            }
            playerHit(col.gameObject);
        }
        else if (col.collider.tag == targetTag)
        {
            // Take Trophy
            currentItem = col.gameObject;
            takeTrophy();
            personnalBehavior();
        }
    }

    IEnumerator speedBuff()
    {
        agent.speed *= speedMulti;
        yield return new WaitForSeconds(speedTime);
        agent.speed = agent.speed/speedMulti;
    }

    protected virtual void takeTrophy()
    {
        if (trophy.parent.parent.name != "PoliceHand" && canSeeObject(trophy.gameObject))
        {
            currentItem.transform.parent = hand;
            currentItem.transform.position = hand.position + (transform.forward * currentItem.GetComponent<Item>().offsetHolding);
            currentItem.transform.rotation = transform.rotation;
            currentItem.GetComponentInParent<Rigidbody>().isKinematic = true;
            currentItem.GetComponentInParent<Item>().Stop();
        }
    }

    protected bool canSeeObject(GameObject target)
    {
        bool canSee = false;
        RaycastHit hit;
        Vector3 dir = target.transform.position - transform.position;

        if (Physics.Raycast(transform.position, dir, out hit))
        {
            if (hit.collider.tag == target.tag)
            {
                canSee = true;
            }
        }

        return canSee;
    }

    public void dropItem()
    {

        if (currentItem == null)
            return;
        if (!currentItem.GetComponentInParent<Enemy>())
        {
            currentItem = null;
            return;
        }


        currentItem.GetComponent<Rigidbody>().isKinematic = false;
        currentItem.GetComponent<Item>().Throw(dropItemForce);

        currentItem.transform.parent = GameObject.Find("ThrowingProps").transform;
        currentItem = null;
    }
}
