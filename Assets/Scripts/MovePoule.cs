using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePoule : MonoBehaviour
{
  private UnityEngine.AI.NavMeshAgent agent;
  private UnityEngine.AI.NavMeshHit hit;
  public float range = 5.0f;
  public float sightRange = 15.0f;
  public float sightAngle = 170.0f;
  public float pondAppetit = 0.5f ;
  public float pondPeur = 0.5f;

  //public Vector3 point;
  private Vector3 direction;
  private Vector3 distance ;
  private Vector3 destination ;
  private Vector3 directionRay ;
  private float offsetFromWall = 4.0f ;
  private Vector3 offset = new Vector3 (-10.0f, 0.0f, -10.0f) ;


  private bool prisEnChasse ;
  private bool enChasse ;
  [HideInInspector] // Hides var below
  public static bool touched ;


  private GameObject[] predatorList ;
  private GameObject predator ;

  private GameObject[] preyList ;
  private GameObject prey ;

  private string tagPrey = "Vipere1";
  private string tagPredator = "Renard1";

  private Vector3 homeVipere = new Vector3( 0.0f,  15.0f, 0.0f);
  private Vector3 homePoule = new Vector3( 15.0f,  0.0f, 0.0f);



    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        predatorList = GameObject.FindGameObjectsWithTag(tagPredator);
        preyList = GameObject.FindGameObjectsWithTag(tagPrey);
        //Debug.Log("preyList = " + preyList.Length);
        enChasse = false;
        prisEnChasse = false;
        touched = false ;
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
           LayerMask mask = LayerMask.GetMask("Wall");
           destination = agent.transform.position + goal ; //le vecteur goal est appliqué depuis la position de l'agent
          // This would cast rays only against colliders in layer 8.
          // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
          float distanceRay = Vector3.Distance(transform.position,destination);
          bool hitForward = Physics.Raycast(agent.transform.position, agent.transform.forward, out RaycastHit hitRayForward, distanceRay, mask);
          bool hitLeft = Physics.Raycast(agent.transform.position, -agent.transform.right, out RaycastHit hitRayLeft, distanceRay, mask) ;
          bool hitRight = Physics.Raycast(agent.transform.position, agent.transform.right, out RaycastHit hitRayRight, distanceRay, mask) ;

          if (hitForward)
          {
            Debug.Log("Wall straight ahead ");
            if (hitRight)
            {
              if (hitLeft && (hitRayLeft.distance < hitRayRight.distance ))
              {
                Debug.Log("Wall to the right as well ");
                destination = goal + agent.transform.position + agent.transform.right * offsetFromWall * 2.0f;
              }
              else
              {
                Debug.Log("Wall to the right as well ");
                destination = goal + agent.transform.position - agent.transform.right * offsetFromWall * 2.0f;
              }
            }
            else if (hitLeft)
            {
                Debug.Log("Wall to the left as well ");
                destination = goal + agent.transform.position + agent.transform.right * offsetFromWall;
            }

          }
          else if (hitRight)
          {
            destination = goal + agent.transform.position - agent.transform.right * offsetFromWall;
          }

          else if (hitLeft)
          {
            destination = goal + agent.transform.position + agent.transform.right * offsetFromWall;
          }
            Debug.DrawRay(destination, Vector3.up, Color.red, 1.0f);
            agent.SetDestination(destination);
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

    bool CibleEnVue(out Vector3 result)
    {
      if(preyList.Length != 0){  // test seulement dans le cas ou la poule est seule (test unitaire)
        var distancePrey = ( transform.position - preyList[0].transform.position ).magnitude;
        //result = preyList[0].transform.position;
        //System.Array.clear(preyVisibleList,0,preyVisibleList.length);
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
      else
      { result = new Vector3 (0.0f,0.0f,0.0f);
        enChasse=false;
        return enChasse;}
    }

    Transform getClosest(List<GameObject> preyVisibleList)
    {
      if(preyVisibleList.Count != 0){
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
              result = preyVisible.transform ; // on définie la proie la plus proche
            }
          }
          // resultat final
        return result;
      }
      else
      { var result = transform;
        return result; }
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
                    //print("Oh god une vipère");
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
          if ((agent.remainingDistance <= agent.stoppingDistance)) // si l'agent est immobile
            {
              if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
              // Bit shift the index of the layer (8) to get a bit mask
                int layerMask = 1 << 8;
                // This would cast rays only against colliders in layer 8.
                // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
                layerMask = ~layerMask;
                RaycastHit hit;
                float distanceRay = Vector3.Distance(transform.position,point);
                Vector3 directionRay = point - transform.position;
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(transform.position, directionRay, out hit, distanceRay, layerMask) == false)
                {
                  Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                  agent.SetDestination(point);
                }
                else
                {
                  print("ray touched") ;
                }
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
          collision.gameObject.transform.position = homeVipere ;
          MoveVipere.touched = true ;
        }
        else if (collision.gameObject.tag=="Wall")
            {
              //agent.SetDestination(new Vector3(0,0,0));
              // Destroy(collision.gameObject);
            Debug.Log("Mur touché : ");

            }
    }

    // Update is called once per frame
    void Update()
    {
      if (touched == true)
      {
        agent.isStopped = touched;
        //agent.SetDestination(homePoule);
        agent.ResetPath();
        touched = false ;
        enChasse = false;
        prisEnChasse = false;
        agent.isStopped = touched;
       }
      predatorList = GameObject.FindGameObjectsWithTag(tagPredator);
      preyList = GameObject.FindGameObjectsWithTag(tagPrey);
      enChasse = CibleEnVue(out Vector3 preyPosition);
      prisEnChasse = prisPourCible(out Vector3 predatorPosition);
 //Debug.Log("prisEnChasse : " + prisEnChasse);
//Debug.Log("enChasse : " + enChasse);
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
