﻿using UnityEngine;
using System.Collections;

public class EnemyAttacker : Enemy {
    enum State
    {
        rand,
        player,
        trophy,
        station
    }
    GameObject[] players;
    public float xTime, zTime, viewDistance;

    protected override void Start()
    {
        
        players = GameObject.FindGameObjectsWithTag("Player");
        currentTarget = players[0].transform;
        getRandomTarget();
        base.Start();
    }

    protected override void personnalBehavior()
    {
        if (currentItem != null && hand.GetComponentInChildren<Item>())
        {
            StopAllCoroutines();
            currentTarget = policeStation;
        }
        else if (currentTarget == policeStation)
        {
            getRandomTarget();
        }
        else
        {
            currentItem = null;
            if (Vector3.Distance(trophy.position, transform.position) <= viewDistance&& trophy.GetComponentInParent<Enemy>()==null)
            {
                followTrophy();
            }
            else
            {
                if (currentTarget == null)
                    currentTarget = trophy;
                if (Vector3.Distance(transform.position, currentTarget.position) > viewDistance)
                {
                    foreach (GameObject player in players)
                    {
                        if (Vector3.Distance(player.transform.position, transform.position) <= viewDistance && !player.GetComponentInParent<Stun>().isStun)
                        {
                            StopAllCoroutines();
                            currentTarget = player.transform;
                            StartCoroutine(cooldown(zTime));
                            break;
                        }
                    }
                }
            }
        }
        base.personnalBehavior();
    }

    protected override void playerHit(GameObject playerHit)
    {
        StopCoroutine("cooldown");
        getRandomTarget();
        base.playerHit(playerHit);
    }

    void followTrophy()
    {
        StopCoroutine("cooldown");
        currentTarget = trophy;
        StartCoroutine(cooldown(zTime * 1.5f));
    }

    void getRandomTarget()
    {
        currentTarget = players[Random.Range(0, players.Length)].transform;
        if (currentTarget.GetComponentInParent<Stun>().isStun)
        {
            foreach (GameObject player in players)
            {
                if (!player.GetComponentInParent<Stun>().isStun)
                {
                    currentTarget = player.transform;
                    break;
                }
                else
                {
                   currentTarget = trophy;
                }
            }
        }
        StartCoroutine(cooldown(xTime));
    }

    IEnumerator cooldown(float t)
    {
        yield return new WaitForSeconds(t);
        
        if (!GetComponent<Stun>().isStun)
        {
            agent.Stop();
            anim.Play("anim_policiers_Idle");
        }
            
        yield return new WaitForSeconds(waitingTime);
        if (!GetComponent<Stun>().isStun)
        {
            agent.Resume();
            anim.Play("anim_policiers_Run");
        }
        getRandomTarget();
    }
    
}
