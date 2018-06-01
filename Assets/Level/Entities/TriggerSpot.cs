using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListOfConnections
{
    public List<Spot> connections;
}

public class TriggerSpot : Spot {
    public enum When { Enter, Leave, Exit };
    public When when = When.Enter;
    public List<Spot> platforms = new List<Spot>();
    public List<ListOfConnections> connectionSpots = new List<ListOfConnections>() ;
    public AnimationCurve curve;
    public Spot triggerSpot;

    private Spot movedUpSpot;
    private Spot movedDownSpot;
    private List<Spot> connectionsBySpot;
    public Player player;
    private bool moveUp;
    public bool up;
    public float topPosition = 4.8f;

    void Start()
    {
        /*while (!Level.instance.bridgesDrawn)
            yield return null;
        */switch (when)
        {
            case When.Enter:
                OnEnter += MovePlatform;
                break;
        }
    }

    void MovePlatform()
    {
        for (int i = 0; i < platforms.Count; ++i) {
            moveUp = platforms[i].transform.position.y < topPosition;
            Debug.Log("Move");
            if (moveUp)
            {
                movedUpSpot = platforms[i];
                connectionsBySpot = connectionSpots[i].connections;
                Vector3 endPos = new Vector3(platforms[i].transform.position.x, platforms[i].transform.position.y + topPosition, platforms[i].transform.position.z);
                Move(platforms[i], 1f, endPos, curve, delegate
                {
                    for (int j = 0; j < connectionsBySpot.Count; ++j)
                    {
                        movedUpSpot.AddBridge(connectionsBySpot[j]);
                    }
                });
                moveUp = false;
            }
            else
            {
                movedDownSpot = platforms[i];
                Vector3 endPos = new Vector3(platforms[i].transform.position.x, platforms[i].transform.position.y - topPosition, platforms[i].transform.position.z);
                Move(platforms[i], 1f, endPos, curve);
                movedDownSpot.DestroyAllBridges();
                moveUp = true;
            }
        }
    }

    public virtual void Move(Spot spot, float duration, Vector3 endPos, AnimationCurve curve, Action endAction = null)
    {
        if (duration != 0f)
        {
            StartCoroutine(MoveRoutine(spot, 1f, endPos, curve, endAction));
        }
    }

    protected virtual IEnumerator MoveRoutine(Spot spot, float duration, Vector3 endPos, AnimationCurve curve, Action endAction = null)
    {
        float t = 0f;
        Vector3 startPos = spot.transform.position;
        while (t < duration)
        {
            yield return null;
            t += Time.deltaTime;
            spot.transform.position = Vector3.Lerp(startPos, endPos, curve.Evaluate(t / duration));
        }
        if (endAction != null)
            endAction();
    }
}
