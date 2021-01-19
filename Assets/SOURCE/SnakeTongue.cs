using UnityEngine;
using System;
using System.Collections;

public class SnakeTongue : MonoBehaviour
{
    public bool isPulling { get; private set; }

    LineRenderer tongeRenderer;
    public AudioClip pullSound;
    public AudioClip eatSound;
    public LayerMask shootableMask;

    GameObject pullingTarget;

    public void Awake()
    {
        tongeRenderer = GetComponent<LineRenderer>();
    }

    void OnDestroy()
    {
        if (pullingTarget != null)
            Destroy(pullingTarget.gameObject);
    }

    public void PullObject(GameObject gameObject, Action OnFinish)
    {
        AudioSource.PlayClipAtPoint(pullSound, transform.position);
        StartCoroutine(AnimateTonguePulling(gameObject, OnFinish));
    }

    public GameObject GetClosestColletibleInRange()
    {
        GameObject collectible = null;

        if (GetPullablesInRange().Length > 0)
        {
            if (GetPullablesInRange()[0].gameObject.tag == "Collectible")
                collectible = GetPullablesInRange()[0].gameObject;
        }

        return collectible;
    }

    public SnakeNode GetClosestNodeInRange()
    {
        SnakeNode node = null;

        if (GetPullablesInRange().Length > 0)
        {
            SnakeNode foundNode = GetPullablesInRange()[0].GetComponent<SnakeNode>();

            if (CanPullNode(foundNode))
                node = foundNode;
        }

        return node;
    }

    bool CanPullNode(SnakeNode node)
    {
        if (node == null)
            return false;

        if (node is Snake)
            return false;

        if (node.snake == null)
            return true;
        else
            return node.snake.isInvincible == false;
    }

    public Collider[] GetPullablesInRange()
    {
        Collider[] foundPullables = Physics.OverlapSphere(transform.position + transform.forward * 4.5f, 4, shootableMask);
        return foundPullables;
    }

    public void MissPulling()
    {
        StartCoroutine(AnimateMissPull());
    }

    private IEnumerator AnimateMissPull()
    {
        float timer = 0;

        while (timer < 2)
        {
            timer += Time.deltaTime * 6;
            SetTongeExtension(Mathf.PingPong(timer, 1), transform.position + transform.forward * 3);
            yield return new WaitForEndOfFrame();
        }

        tongeRenderer.SetPosition(2, Vector3.zero);

    }

    private IEnumerator AnimateTonguePulling(GameObject target, Action OnFinishPull)
    {
        isPulling = true;

        float timer = 0;
        pullingTarget = target;
        Destroy(target.GetComponent<Collider>());

        while (timer < 2 && target != null)
        {
            timer += Time.deltaTime * 6;

            SetTongeExtension(Mathf.PingPong(timer, 1), target.transform.position);

            if (timer >= 1)
            {
                target.transform.position = Vector3.Lerp(transform.position, target.transform.position, Mathf.PingPong(timer, 1));
            }

            yield return new WaitForEndOfFrame();
        }

        Destroy(target.gameObject);
        tongeRenderer.SetPosition(2, Vector3.zero);
        isPulling = false;

        OnFinishPull();
    }

    void SetTongeExtension(float extendedAmount, Vector3 position)
    {
        Vector3 tongeExtendPosition = transform.InverseTransformPoint(position);
        Vector3 tongePosition = Vector3.Lerp(Vector3.zero, tongeExtendPosition, extendedAmount);
        tongeRenderer.SetPosition(2, tongePosition);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 4.5f, 4);
    }

}