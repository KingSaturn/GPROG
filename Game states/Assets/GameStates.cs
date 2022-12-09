using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class AIState
{
    public virtual void stateUpdate(GameStates thisObject) { }
};
public class AlertState : AIState
{
    private float lastTime = Time.time;
    public override void stateUpdate(GameStates thisObject)
    {
        if(thisObject.distToPlayer < thisObject.viewDistance)
        {
            lastTime = Time.time;
        }
        if (thisObject.distToPlayer < thisObject.viewDistance)
        {
            thisObject.currentState = new ChaseState();
            Debug.Log("Chase!");
        }
        if(thisObject.distToPlayer > thisObject.viewDistance)
        {
            var timeSince = Time.time - lastTime;
            if (timeSince > thisObject.focusTime)
            {
                thisObject.currentState = new ReturnToStart();
                Debug.Log("Return");
            }
        }
    }
}
public class ShootingState : AIState
{
    private RaycastHit hitPlayer;
    public override void stateUpdate(GameStates thisObject)
    {
        thisObject.shootingState = true;
        thisObject.transform.LookAt(thisObject.playerObject.transform.position);
        Vector3 newPos = thisObject.transform.position;
        if (thisObject.distToPlayer < thisObject.runDistance & Input.GetKeyUp(thisObject.swapStateKey))
        {
            thisObject.currentState = new RunAway();
            Debug.Log("Run");
        }
        if (Physics.Raycast(newPos, thisObject.transform.forward, out hitPlayer, thisObject.gunRange))
        {
            thisObject.playerHit = true;
        }
        if(thisObject.distToPlayer > thisObject.gunRange)
        {
            thisObject.currentState = new ChaseState();
        }
    }
}
public class ChaseState : AIState
{
    private readonly float runSpeed = 5.0f;
    private float lastTime = Time.time;
    public override void stateUpdate(GameStates thisObject)
    {
        thisObject.shootingState = false;
        thisObject.transform.LookAt(thisObject.playerObject.transform.position);
        Vector3 NewPos = thisObject.transform.position + (runSpeed * Time.deltaTime * thisObject.transform.forward.normalized);
        thisObject.transform.position = NewPos;
        if (thisObject.distToPlayer < thisObject.runDistance & Input.GetKeyUp(thisObject.swapStateKey))
        {
            thisObject.currentState = new RunAway();
            Debug.Log("Run");
        }
        if(thisObject.distToPlayer < thisObject.gunRange)
        {
            thisObject.currentState = new ShootingState();
        }
        if (thisObject.distToPlayer < thisObject.viewDistance)
        {
            lastTime = Time.time;
            Debug.Log("Shoot");
        }
        if (thisObject.distToPlayer > thisObject.viewDistance)
        {
            var timeSince = Time.time - lastTime;
            if (timeSince > thisObject.focusTime)
            {
                thisObject.currentState = new AlertState();
                Debug.Log("Alert");
            }
        }
    }
}
public class RunAway : AIState
{
    public float walkspeed = 5.0f;
    public override void stateUpdate(GameStates thisObject)
    {
        thisObject.shootingState = false;
        thisObject.transform.LookAt(thisObject.playerObject.transform.position);
        Vector3 NewPos = thisObject.transform.position - (thisObject.transform.forward.normalized * walkspeed * Time.deltaTime);
        thisObject.transform.position = NewPos;
        if (thisObject.distToPlayer > thisObject.runDistance)
        {
            thisObject.currentState = new ReturnToStart();
            Debug.Log("Return");
        }
    }
}
public class ReturnToStart : AIState
{
    public float walkSpeed = 2.0f;
    public override void stateUpdate(GameStates thisObject)
    {
        thisObject.transform.LookAt(thisObject.pointB.transform.position);
        Vector3 NewPos = thisObject.transform.position + (thisObject.transform.forward.normalized * walkSpeed * Time.deltaTime);
        thisObject.transform.position = NewPos;

        if (thisObject.distToPlayer < thisObject.viewDistance)
        {
            thisObject.currentState = new AlertState();
            Debug.Log("Alert");
        }
        else if (Vector3.Distance(thisObject.transform.position, thisObject.pointB.transform.position) <= 0.5f)
        {
            thisObject.currentState = new PatrolState();
            Debug.Log("Patrol");
            thisObject.transform.LookAt(thisObject.pointA.transform.position);
        }
    }
}
public class PatrolState : AIState
{
    public float walkSpeed = 2.0f;
    public float pointADist;
    public float pointBDist;
    public override void stateUpdate(GameStates thisObject)
    {
        pointADist = Vector3.Distance(thisObject.pointA.transform.position, thisObject.transform.position);
        pointBDist = Vector3.Distance(thisObject.pointB.transform.position, thisObject.transform.position);
        float threshold = 0.5f;
        Vector3 NewPos = thisObject.transform.position + (thisObject.transform.forward.normalized * walkSpeed * Time.deltaTime);
        thisObject.transform.position = NewPos;
        if (pointADist <= threshold)
        {
            thisObject.transform.LookAt(thisObject.pointB.transform.position);
        }
        if (pointBDist <= threshold)
        {
            thisObject.transform.LookAt(thisObject.pointA.transform.position);
        }
        if (thisObject.distToPlayer < thisObject.viewDistance)
        {
            thisObject.currentState = new AlertState();
            Debug.Log("Alert");
        }
        
    }
}
public class DeadState : AIState
{
    public override void stateUpdate(GameStates thisObject)
    {
        thisObject.shootingState = false;
        Rigidbody rBody = thisObject.GetComponent<Rigidbody>();
        Renderer r = thisObject.GetComponent<Renderer>();
        rBody.isKinematic = false;
        r.material.color = Color.yellow;
    }
}
public class GameStates : MonoBehaviour
{
    public PlayerController playercont;
    private Collider coll;
    public Camera cameraA;
    [System.NonSerialized]public float rayLength;
    public GameObject playerObject;
    public float viewDistance;
    public float focusTime;
    public KeyCode swapStateKey;
    public int gunRange;
    public float runDistance;
    public GameObject pointA;
    public GameObject pointB;
    [System.NonSerialized] public Vector3 pos;
    public AIState currentState;
    [System.NonSerialized] public bool shootingState = false;
    [System.NonSerialized] public float distToPlayer;
    [System.NonSerialized] public bool playerHit = false;
    void Start()
    {
        rayLength = 99.0F;
        coll = GetComponent<Collider>();
        pos = transform.position;
        this.transform.LookAt(pointA.transform.position);
        currentState = new PatrolState();
        InvokeRepeating("Report", 0.0f, 3.0f);
    }
    void Update()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            Ray myRay = cameraA.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitObject;
            if(coll.Raycast(myRay, out hitObject,rayLength))
            {
                currentState = new DeadState();
            }
        }
        distToPlayer = Vector3.Distance(playerObject.transform.position, transform.position);
        currentState.stateUpdate(this);
    }
    void Report()
    {
        if (shootingState == true)
        {
            Debug.Log("ping");  
            damage(this);
        }
    }
    void damage(GameStates thisObject)
    {
        if(playerHit == true)
        {
            thisObject.playercont.health--;
        }    
    }
}
