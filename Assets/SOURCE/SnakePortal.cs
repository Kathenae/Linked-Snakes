using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class SnakePortal : MonoBehaviour
{
    public bool isExit { get; set; }
    public Snake traveler { get; set; } //The snake that isLocked traveling throught this portal

    public SnakePortal otherEnd;
    public AudioClip enterSound;
    public AudioClip exitSound;

    OffMeshLink offMeshLink;

    void Start()
    {
        offMeshLink = GetComponent<OffMeshLink>();
    }

    void Update()
    {

    }

    public List<SnakeNode> nodesThatPassed;

    void OnTriggerStay(Collider collider)
    {

        if (otherEnd == null)
        {
            Debug.LogError("This portal does not have an exit, did you forget to set it in the inspector?");
            return;
        }

        // BUG: Sometimes somehow the tail doesn't go throught the portal, We need to make sure to detect that
        // and reativate the portal for other snake's to go throught 

        // BUG: Sometimes when there's a snake at the other end portal
        // that snake will be propeled upwards when something exits on that portal

        // Don't do anything if something is exiting out of this portal
        if (isExit)
            return;

        Snake snake = collider.GetComponent<Snake>();

        //If the colliding object is a snake and this portal has no traveler
        if (snake != null && traveler == null)
        {
            //Debug.Log("Travel Started"); 

            // Make the traveler the snake
            traveler = snake;
            otherEnd.traveler = snake;

            // Make the snakes next node move to this portal
            traveler.next.target = gameObject;
            // Teleport the snake to the other portal
            TeleportToExit(snake.gameObject);
            // Start the link by setting the other portal to be the exit portal
            StartLink();

            nodesThatPassed = new List<SnakeNode>();
            nodesThatPassed.Add(snake);

            return;
        }

        SnakeNode node = collider.GetComponent<SnakeNode>();

        if (traveler == null || traveler == snake || nodesThatPassed.Contains(node))
            return;

        // If the snake traveling throught this portal is the snake the node is part of, 
        // And if the previous node of this node has already passed throught this portal
        if (traveler == node.snake && nodesThatPassed.Contains(node.previous))
        {

            //Make the follower of the node move to this portal
            if (node.next != null)
                node.next.target = gameObject;

            //Teleport the node to the exit portal
            TeleportToExit(node.gameObject);
            nodesThatPassed.Add(node);

            // Make the node move to its real target 
            // (Since it was made to move to the portal by its previous node)
            if (node.previous != null)
                node.target = node.previous.gameObject;

        }

    }

    void OnTriggerExit(Collider c)
    {
        //Assuming we're the exit portal
        if (isExit)
        {
            SnakeNode node = c.GetComponent<SnakeNode>();
            //If the node is the tail of the snake traveling throught this portal link
            if (node == otherEnd.traveler.tail)
            {
                // The snake finished traveling throught this portal
                // Debug.Log("Portal Travel Finished");

                // So unlock the portal link
                EndLink();

                // And clear the link for other travelers
                otherEnd.traveler = null;
                traveler = null;
                nodesThatPassed = null;
            }

            // If a node exits through this portal and its an enemy snake(AI)
            // We have to recalculate its path
            if (node is EnemySnake)
            {
                //BUG: sometimes portal looping happens when the enemy destination is near a portal
                // Can be fixed by making sure to place portals in a way that they don't have a straight path to each other
                EnemySnake snakeAsEnemy = (EnemySnake)node;
                snakeAsEnemy.Recalculatepath();
            }

        }
    }

    void TeleportToExit(GameObject traveler)
    {
        StartCoroutine(AnimateExit(traveler, traveler.transform.position));
    }

    IEnumerator AnimateExit(GameObject node, Vector3 enterPosition)
    {
        float timer = 0;
        bool teleported = false;
        while (timer < 2 && node != null)
        {
            timer += Time.deltaTime * 3;
            Vector3 scale = Vector3.Lerp(Vector3.one, Vector3.zero, Mathf.PingPong(timer, 1));
            node.transform.localScale = scale;

            if (timer >= 1 && !teleported)
            {
                node.transform.position = otherEnd.transform.position;
                teleported = true;
            }
            else if (!teleported)
            {
                node.transform.position = enterPosition;
            }

            yield return new WaitForEndOfFrame();
        }

        if (node != null)
            node.transform.localScale = Vector3.one;

    }

    void StartLink()
    {
        SetOffMeshLinkActiveted(false);
        otherEnd.SetOffMeshLinkActiveted(false);

        otherEnd.isExit = true;
        traveler.isInvincible = true;
        otherEnd.GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        
        if(enterSound != null)
            AudioSource.PlayClipAtPoint(enterSound, transform.position);
    }

    void EndLink()
    {
        SetOffMeshLinkActiveted(true);
        otherEnd.SetOffMeshLinkActiveted(true);

        isExit = false;
        traveler.isInvincible = false;
        otherEnd.gameObject.GetComponent<MeshRenderer>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;

        if(exitSound != null)
            AudioSource.PlayClipAtPoint(exitSound, transform.position);
    }

    void SetOffMeshLinkActiveted(bool value)
    {
        if (offMeshLink != null)
            offMeshLink.activated = value;
    }

}
