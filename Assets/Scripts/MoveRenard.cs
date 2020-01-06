using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRenard : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    private UnityEngine.AI.NavMeshHit hit;
    private float range = 10.0f;
    private float sightRange = 20.0f;
    private float sightAngle = 180.0f;
    public Vector3 point;
    private Vector3 direction;
    private Vector3 distance ;
    private bool enChasse = false;

    private GameObject[] vipereList ;
    private GameObject vipere ;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        vipereList = GameObject.FindGameObjectsWithTag("Vipere1");
        vipere = vipereList[0];
        Vector3 point = vipere.transform.position;
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
    
    void RunAway(Vector3 point)
    {
        if (AwayPoint(point, range, out Vector3 goal))
        {
            Debug.DrawRay(agent.transform.position + goal, Vector3.up, Color.red, 1.0f);
            agent.SetDestination(agent.transform.position + goal); //le vecteur goal est appliqué depuis la position de l'agent
        }
    }
    bool AwayPoint(Vector3 predator, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
          // Gets a vector that points from the player's position to the target's.
           distance = predator - transform.position;
           direction = distance.normalized;
           Vector3 cible = - direction * range;
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(cible, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    bool CibleVisee(out Vector3 result)
    {
        for (int i = 0; i < vipereList.Length; i++)
        {
            GameObject vipere = vipereList[i];
            if ( (vipere.transform.position - agent.transform.position).magnitude < sightRange)
            {
                Vector3 cibleDir = vipere.transform.position - agent.transform.position;
                if (Vector3.Angle(cibleDir, agent.transform.forward) < sightAngle)
                {
                    result = vipere.transform.position;
                    print("Oh god une vipère");
                    prisEnChasse = true;
                    return true;
                }
            }
        }
        result = Vector3.zero;
        return false;
    }

    /*void Chase()
    {
        if ()
    }
    */



    void FreeWalk()
    {
        if (RandomPoint(transform.position, range, out point))
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
            agent.SetDestination(point);
        }
    }


    // Update is called once per frame
    void Update()
    {
      vipereList = GameObject.FindGameObjectsWithTag("Vipere1");
      vipere = vipereList[0];
      Vector3 point = vipere.transform.position;

      if(CibleVisee(out point))
      {
              RunAway(point);
      }
      else
      {
          if (!agent.pathPending)
          {
              if (agent.remainingDistance <= agent.stoppingDistance) // si l'agent est immobile
              {
                  if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                  {
                        FreeWalk() ;
                  }
              }
          }
      }

    }
}
