using System.Collections;
using System;
using UnityEngine;

[RequireComponent(typeof(SnakeTongue))]
public class Snake : SnakeNode
{

    Transform container;
    public SnakeNode nodePrefab;
    public AudioClip dieSound;


    public int startingLength;
    public float speed;
    public bool isInvincible;
    private int length;

    public SnakeNode tail { get; set; }
    public SnakeTongue tongue { get; set; }


    protected virtual void Awake()
    {
        tongue = GetComponent<SnakeTongue>();
        Initiate();
    }

    public void Initiate()
    {
        container = new GameObject(name).transform;
        name = "Head";
        transform.parent = container;

        CreateTail();
        Grow(startingLength);
    }

    void CreateTail()
    {
        tail = SpawnNodeAtTail();

        next = tail;
        tail.snake = this;
        tail.previous = this;
        length += 1;
    }

    public void Grow(int amount = 1)
    {
        if (amount <= 0)
            return;

       PlayGrowAnimation(OnFinished: () =>
       {
           for (int i = 0; i < amount; i++)
           {
               GrowOneNode();
           }
       });
    }

    private void GrowOneNode()
    {
        SnakeNode grownNode = SpawnNodeAtTail();
        grownNode.snake = this;
        
        if(tail != null)
        {
            tail.next = grownNode;
            tail.name = "Body";
            grownNode.previous = tail;
        }

        grownNode.snake = this;
        grownNode.name = "Tail";
        tail = grownNode;

        length += 1;
    }

    public void Pull()
    {
        if (tongue.isPulling || isInvincible)
            return;

        GameObject collectible = tongue.GetClosestColletibleInRange();
        SnakeNode node = tongue.GetClosestNodeInRange();

        if (collectible != null)
        {
            tongue.PullObject(collectible, OnFinish: () =>
            {
                Grow();
                GameEvents.TriggerCollectibleEaten(this,collectible);
            });

            return;
        }

        if (node != null)
        {
            node.Unlink(OnSnakeDeath: () =>
            {
                Grow(3);
            });

            tongue.PullObject(node.gameObject, OnFinish: () =>
            {
                GameEvents.TriggerNodeStolen(this, node.snake);
                PlayGrowAnimation();

                // amountEatenOfOther += 1;
                //
                //if (amountEatenOfOther >= amountNeededToGrowEatingOther)
                //{
                //    amountEatenOfOther = 0;
                //    Grow(amountGrownEatingOther);
                //}
                //else
                //{
                //    PlayGrowAnimation();
                //}

            });

            return;
        }

        tongue.MissPulling();

    }

    SnakeNode SpawnNodeAtTail()
    {
        Vector3 position = tail != null ? tail.transform.position : transform.position;
        SnakeNode node = Instantiate(nodePrefab, position, Quaternion.identity);
        node.transform.parent = container;
        return node;
    }

    public void PlayGrowAnimation(Action OnFinished = null)
    {
        StartCoroutine(AnimateGrowing(OnFinished));
    }

    private IEnumerator AnimateGrowing(Action OnFinished = null)
    {
        SnakeNode nextNode = next;
        Color bodyColor = GetComponent<MeshRenderer>().material.color;
        while (nextNode != null)
        {
            nextNode.transform.localScale = Vector3.one * 1.45f;
            nextNode.GetComponent<MeshRenderer>().material.color = bodyColor;
            yield return new WaitForSeconds(0.02f);
            nextNode.GetComponent<MeshRenderer>().material.color = bodyColor;
            nextNode.transform.localScale = Vector3.one;
            nextNode = nextNode.next;
        }

        AudioSource.PlayClipAtPoint(GetComponent<SnakeTongue>().eatSound, transform.position, 5);

        if (OnFinished != null)
            OnFinished();
    }


    public void TakeDamage(Action OnDeath)
    {
        length -= 1;

        if (length <= 0)
        {
            if(OnDeath != null)
                OnDeath();

            Die();
        }
    }

    public void UnlinkNode(SnakeNode node)
    {
        if (node == tail)
        {
            node.previous.name = "Tail";
            tail = node.previous;
        }

        node.previous.next = node.next;

        if (node.next != null)
        {
            node.next.previous = node.previous;
            node.next.target = node.target;
            node.SwapPosition(node.transform.position);
        }

        node.transform.parent = null;
        node.previous = null;
        node.next = null;
        node.snake = null;

    }

    void Die()
    {
        GameEvents.TriggerSnakeDeath(this);
        AudioSource.PlayClipAtPoint(dieSound, transform.position,20);
        Destroy(gameObject);
        Destroy(transform.parent.gameObject);
    }

}
