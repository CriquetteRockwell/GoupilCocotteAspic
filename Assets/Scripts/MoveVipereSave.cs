using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveVipereSave : MonoBehaviour
{
    //string leftvalue = "peur";
  //  string rightvalue = "affamé";
  //  public static float Slider(float value, string leftValue, string rightValue);
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform prey;
    private Transform predator;

    public Vector3 point;
    private Vector3 direction;
    private Vector3 distance ;
    private bool prisEnChasse ;

    private float sightRange = 20.0f;
    private float sightAngle = 180.0f;

    private GameObject[] renardList ;
    private GameObject[] pouleList ;

    private GameObject renard1 ;
    private GameObject poule1 ;
    public Vector3 randomPoint;
    private float range = 10.0f;
    private Vector3 home = new Vector3(- 0.0f, - 0.0f, 0.0f);
    private Vector3 destination ;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        renardList = GameObject.FindGameObjectsWithTag("Renard1");
        renard1 = renardList[0];
        Vector3 point = renard1.transform.position;
        prey = renard1.transform;

        pouleList = GameObject.FindGameObjectsWithTag("Poule1");
        poule1 = pouleList[0];
        predator = poule1.transform;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        randomPoint = transform.position + Random.insideUnitSphere * range;

    }

  bool CibleVisee(out Vector3 result)
      {
          for (int i = 0; i < pouleList.Length; i++)
          {
              GameObject poule = pouleList[i];
              if ( (poule.transform.position - agent.transform.position).magnitude < sightRange)
              {
                  Vector3 cibleDir = poule.transform.position - agent.transform.position;
                  if (Vector3.Angle(cibleDir, agent.transform.forward) < sightAngle)
                  {
                      result = poule.transform.position;
                      print("Oh god une poule");
                      prisEnChasse = true;
                      return true;
                  }
              }
          }
          result = Vector3.zero;
          return false;
      }

      void RunAway(Vector3 point)
      {
          if (AwayPoint(point, range, out Vector3 goal))
          {
            int layerMask = 1 << 8;
            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;
            RaycastHit hit;
            float distanceRay = Vector3.Distance(transform.position,goal);
            Vector3 directionRay = goal - transform.position;
            while (Physics.Raycast(transform.position, directionRay, out hit, distanceRay, layerMask))
            {
              goal = goal + new Vector3(1.0f, 0.0f, 0.0f) ;
              distanceRay = Vector3.Distance(transform.position,goal);
              directionRay = goal - transform.position;
            }
              destination = agent.transform.position + goal ; //le vecteur goal est appliqué depuis la position de l'agent
              Debug.DrawRay(agent.transform.position + goal, Vector3.up, Color.red, 1.0f);
              agent.SetDestination(destination);
          }
      }

  bool AwayPoint(Vector3 predator, float range, out Vector3 result)
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

        result = Vector3.zero;
        return false;
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

    void FreeWalk()
    {
        if (RandomPoint(transform.position, range, out point))
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
            agent.SetDestination(point);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
      if (collision.gameObject.tag=="Renard1")
        {
          //agent.SetDestination(new Vector3(0,0,0));
          // Destroy(collision.gameObject);
          collision.gameObject.transform.position = home ;
        }
    }

    // Update is called once per frame
    void Update()
    {
        renardList = GameObject.FindGameObjectsWithTag("Renard1");

        // on initialise la référence distance
        var ecartRenard = transform.position - renardList[0].transform.position;
        var distanceRenard = ecartRenard.magnitude;
        // suivre la proie
        // on vérifie chaque poule pour savoir laquelle est la plus proche
        foreach (GameObject renard in renardList)
        {
          if (Vector3.Distance(transform.position, renard.transform.position) < distanceRenard)
          {
            ecartRenard = transform.position - renard.transform.position ;
            distanceRenard = ecartRenard.magnitude ; // on donne la nouvelle valeur à comparer
            prey=renard.transform ; // on définie la proie la plus proche
          }
        }

        agent.SetDestination(prey.position);
    }
}
