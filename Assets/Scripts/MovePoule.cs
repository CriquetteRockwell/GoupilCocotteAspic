using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRenard : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    private UnityEngine.AI.NavMeshHit hit;
    public float range = 10.0f;
    public float sightRange = 20.0f;
    public float sightAngle = 180.0f;
    public float pondAppetit = 0.5f ;
    public float pondPeur = 0.5f;
    //public Vector3 point;
    private Vector3 direction;
    private Vector3 distance ;
    private bool prisEnChasse ;
    private bool enChasse ;


    private GameObject[] renardList ;
    private GameObject renard ;

    private GameObject[] vipereList ;
    private GameObject vipere ;

    private Vector3 homeVipere ;



    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        renardList = GameObject.FindGameObjectsWithTag("Renard1");
        vipereList = GameObject.FindGameObjectsWithTag("Vipere1");
        homeVipere = new Vector3(- 15.0f, - 15.0f, 0.0f);
        enChasse = false;
        prisEnChasse = false;
        // Vector3 point = renard.transform.position;
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

    void RunAfter(Vector3 point)
    {
          Debug.DrawRay(point, Vector3.up, Color.green, 1.0f);
          agent.SetDestination(point);
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

    bool CibleEnVue(out Vector3 result)
    {
      var distancePrey = ( transform.position - vipereList[0].transform.position ).magnitude;
      result = vipereList[0].transform.position;
        for (int i = 0; i < vipereList.Length; i++)
        {
            GameObject vipere = vipereList[i];
            // on teste si la vipere est a distance de vue  ..................................  et    dans le champ de vision  ...........................................................................    et    s'il n'y a pas une proie plus proche
            if ( ((vipere.transform.position - agent.transform.position).magnitude < sightRange) && (Vector3.Angle(vipere.transform.position - agent.transform.position, agent.transform.forward) < sightAngle) && ((vipere.transform.position - agent.transform.position).magnitude < distancePrey) ) {

              distancePrey = (transform.position - vipere.transform.position ).magnitude ; // on donne la nouvelle valeur à comparer
              result = vipere.transform.position;
              enChasse = true;

            } else  {
              result = Vector3.zero;
              enChasse = false ;
              return false;
            }
        }
      return enChasse;
    }

    /*Vector3 cibleDir = vipere.transform.position - agent.transform.position;
    // on teste si la proie est devant le prédateur
    if (Vector3.Angle(vipere.transform.position - agent.transform.position, agent.transform.forward) < sightAngle)
    {

      print("Oh my god a fat jucy chick");
      // on prend la proie la plus proche
      if ((vipere.transform.position - agent.transform.position).magnitude < distancePrey)
      {
        distancePrey = (transform.position - vipere.transform.position ).magnitude ; // on donne la nouvelle valeur à comparer
        result = vipere.transform.position;
      }
    }
}*/

    bool prisPourCible(out Vector3 result)
    {
        for (int i = 0; i < renardList.Length; i++)
        {
            GameObject renard = renardList[i];
            if ( (renard.transform.position - agent.transform.position).magnitude < sightRange)
            {
                Vector3 cibleDir = renard.transform.position - agent.transform.position;
                if (Vector3.Angle(cibleDir, agent.transform.forward) < sightAngle)
                {
                    result = renard.transform.position;
                    print("Oh god un renard");
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
        if (RandomPoint(transform.position, range, out Vector3 point))
        {
          if ((!agent.pathPending) && (agent.remainingDistance <= agent.stoppingDistance)) // si l'agent est immobile
            {
              if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
              {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                agent.SetDestination(point);
              }
            }
          }
        }

    void OnCollisionEnter(Collision collision)
    {
      if (collision.gameObject.tag=="Vipere1")
        {
          //agent.SetDestination(new Vector3(0,0,0));
          // Destroy(collision.gameObject);
          //collision.gameObject.transform.position = homeVipere ;
          print("just teleported");
        }
    }

    // Update is called once per frame
    void Update()
    {
      renardList = GameObject.FindGameObjectsWithTag("Renard1");
      vipereList = GameObject.FindGameObjectsWithTag("Vipere1");
      enChasse = CibleEnVue(out Vector3 prey);
      prisEnChasse = prisPourCible(out Vector3 predator);

      if( enChasse && prisEnChasse ) {
        if  (Vector3.Distance(prey,transform.position)*pondAppetit > Vector3.Distance(predator,transform.position)*pondPeur) // mode intermédiaire
             {  RunAfter(prey) ; }
        else {  RunAway( predator) ; } }
    else if (enChasse == false && prisEnChasse == false) {    FreeWalk() ; } // mode balade
    else if (enChasse == true && prisEnChasse == false) {   RunAfter( prey) ; } // mode chasse
    else if (enChasse == false && prisEnChasse == true) {   RunAway( predator) ; } // mode fuite


    }
  }
