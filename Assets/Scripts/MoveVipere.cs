using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movepoule : MonoBehaviour
{
    public float speed = 3.0f;
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform prey;
    private Transform predator;
    private Vector3 direction ;


    private GameObject[] renardList ;
    private GameObject[] pouleList ;

    private GameObject renard1 ;
    private GameObject poule1 ;
    public Vector3 randomPoint;
    private float range = 10.0f;
    private Vector3 home = new Vector3(- 0.0f, - 0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        renardList = GameObject.FindGameObjectsWithTag("Renard1");
        renard1 = GameObject.FindWithTag("Renard1");
        prey = renard1.transform;

        pouleList = GameObject.FindGameObjectsWithTag("poule1");
        poule1 = GameObject.FindWithTag("poule1");
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

        float step = speed * Time.deltaTime ; // calculate distance to move
        // on initialise la référence distance
        var ecartChik = transform.position - renardList[0].transform.position;
        var distanceChik = ecartChik.magnitude;
        // suivre la proie
        // on vérifie chaque poule pour savoir laquelle est la plus proche
        foreach (GameObject renard in renardList)
        {
          if (Vector3.Distance(transform.position, renard.transform.position) < distanceChik)
          {
            ecartChik = transform.position - renard.transform.position ;
            distanceChik = ecartChik.magnitude ; // on donne la nouvelle valeur à comparer
            prey=renard.transform ; // on définie la proie la plus proche
          }
        }

        agent.SetDestination(prey.position);
    }
}
