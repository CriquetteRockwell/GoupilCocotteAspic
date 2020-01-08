using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRenard : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    private UnityEngine.AI.NavMeshHit hit;
    public float range = 10.0f;
    public float sightRange = 15.0f;
    public float sightAngle = 170.0f;
    public float pondAppetit = 0.5f ;
    public float pondPeur = 0.5f;

    //public Vector3 point;
    private Vector3 direction;
    private Vector3 distance ;
    private bool prisEnChasse ;
    private bool enChasse ;


    private GameObject[] predatorList ;
    private GameObject predator ;

    private GameObject[] preyList ;
    private GameObject prey ;

    private Vector3 homePoule ;
    private string tagPrey = "Poule1";
    private string tagPredator = "Vipere1";



    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        predatorList = GameObject.FindGameObjectsWithTag(tagPredator);
        preyList = GameObject.FindGameObjectsWithTag(tagPrey);
        homePoule = new Vector3(- 15.0f,  15.0f, 0.0f);
        enChasse = false;
        prisEnChasse = false;
        // Vector3 point = predator.transform.position;
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

    bool AwayPoint(Vector3 predatorPosition, float range, out Vector3 result)
    {
        // Gets a vector that points from the player's position to the target's.
        distance = predatorPosition - transform.position;
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

    Transform getClosest(List<GameObject> preyVisibleList)
    {
      // initialisation
      var ecart = transform.position - preyVisibleList[0].transform.position;
      var distance = ecart.magnitude;
      var result = preyList[0].transform;
      foreach (GameObject preyVisible in preyVisibleList)
      {
          if (Vector3.Distance(transform.position, preyVisible.transform.position) < distance)
          {
            ecart = transform.position - preyVisible.transform.position ;
            distance = ecart.magnitude ; // on donne la nouvelle valeur à comparer
            result=preyVisible.transform ; // on définie la proie la plus proche
          }
        }
        // resultat final
      return result;
    }

    bool CibleEnVue(out Vector3 result)
    {
      var distancePrey = ( transform.position - preyList[0].transform.position ).magnitude;
      result = preyList[0].transform.position;
      List<GameObject> preyVisibleList = new List<GameObject>();
        for (int i = 0; i < preyList.Length; i++)
        {
            GameObject prey = preyList[i];
            // on teste si la prey est a distance de vue  ..................................  et    dans le champ de vision  ...........................................................................    et    s'il n'y a pas une proie plus proche
            if ( ((prey.transform.position - agent.transform.position).magnitude < sightRange) && (Vector3.Angle(prey.transform.position - agent.transform.position, agent.transform.forward) < sightAngle))
              {
                enChasse = true ;
                preyVisibleList.Add(prey) ;

              } else  {

                result = Vector3.zero;
                enChasse = false ;
              }
        }
        Transform tempResult = getClosest(preyVisibleList);
        result = tempResult.position ;
        return enChasse;
    }

    /*Vector3 cibleDir = prey.transform.position - agent.transform.position;
    // on teste si la proie est devant le prédateur
    if (Vector3.Angle(prey.transform.position - agent.transform.position, agent.transform.forward) < sightAngle)
    {

      print("Oh my god a fat jucy chick");
      // on prend la proie la plus proche
      if ((prey.transform.position - agent.transform.position).magnitude < distancePrey)
      {
        distancePrey = (transform.position - prey.transform.position ).magnitude ; // on donne la nouvelle valeur à comparer
        result = prey.transform.position;
      }
    }
}*/

    bool prisPourCible(out Vector3 result)
    {
        for (int i = 0; i < predatorList.Length; i++)
        {
            GameObject predator = predatorList[i];
            if ( (predator.transform.position - agent.transform.position).magnitude < sightRange)
            {
                Vector3 cibleDir = predator.transform.position - agent.transform.position;
                if (Vector3.Angle(cibleDir, agent.transform.forward) < sightAngle)
                {
                    result = predator.transform.position;
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
        if (RandomPoint(transform.position, range, out Vector3 point))
        {
          if (/*(!agent.pathPending) && */(agent.remainingDistance <= agent.stoppingDistance)) // si l'agent est immobile
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
      if (collision.gameObject.tag==tagPrey)
        {
          //agent.SetDestination(new Vector3(0,0,0));
          // Destroy(collision.gameObject);
          collision.gameObject.transform.position = homePoule ;
        }
    }

    // Update is called once per frame
    void Update()
    {
      predatorList = GameObject.FindGameObjectsWithTag(tagPredator);
      preyList = GameObject.FindGameObjectsWithTag(tagPrey);
      enChasse = CibleEnVue(out Vector3 preyPosition);
      prisEnChasse = prisPourCible(out Vector3 predatorPosition);

        if( enChasse && prisEnChasse ) {
          if  (Vector3.Distance(preyPosition,transform.position)*pondAppetit > Vector3.Distance(predatorPosition,transform.position)*pondPeur) // mode intermédiaire
               {  RunAfter(preyPosition) ; }
          else {  RunAway( predatorPosition) ; } }
      else if (enChasse == false && prisEnChasse == false) {    FreeWalk() ; } // mode balade
      else if (enChasse == true && prisEnChasse == false) {   RunAfter( preyPosition) ; } // mode chasse
      else if (enChasse == false && prisEnChasse == true) {   RunAway( predatorPosition) ; } // mode fuite

      // if( enChasse && prisEnChasse ) {
      //   if  (Vector3.Distance(preyPosition,transform.position)*pondAppetit > Vector3.Distance(predatorPosition,transform.position)*pondPeur) // mode intermédiaire
      //        {  RunAfter(preyPosition) ; }
      //   else {  RunAfter(preyPosition) ; } }
      // else if (enChasse == false && prisEnChasse == false) {    RunAfter(preyPosition) ; } // mode balade
      // else if (enChasse == true && prisEnChasse == false) {   RunAfter( preyPosition) ; } // mode chasse
      // else if (enChasse == false && prisEnChasse == true) {   RunAfter(preyPosition) ; } // mode fuite




    }
}
