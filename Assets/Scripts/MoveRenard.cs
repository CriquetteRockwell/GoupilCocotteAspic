using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveRenard : MonoBehaviour
{
  private UnityEngine.AI.NavMeshAgent agent ;
  private UnityEngine.AI.NavMeshHit hit ;
  private float tailleTerrain = 23.0f ;
  public float range = 10.0f ;
  public float sightRange = 10.0f ;
  public float sightAngle = 170.0f;
  public float pondAppetit = 1.0f ;
  public float pondPeur = 1.0f ;
  public float pondAltruist = 1.0f ;
  public float pondEgoist = 1.0f ;

  //public Vector3 point;
  private Vector3 directionRay ;
  private float offsetFromWall = 4.0f ;
  private Vector3 offset = new Vector3 (-10.0f, 0.0f, -10.0f) ;

  [HideInInspector]
  public static GameObject premierRenardArrested ;
  private bool prisEnChasse ;
  private bool enChasse ;
  //[HideInInspector] // Hides var below
  public bool touched ;
  private bool preyTouched ;
  private bool predatorTouched ;
  private bool amiArrete ;
  private bool firstVictim ;
  private bool unCamaradeALiberer ;
  private bool gameOver ;
    [HideInInspector]
  static public bool gameOverRenard ;


  private GameObject[] predatorList ;
  private GameObject predator ;

  private GameObject[] preyList ;
  private GameObject prey ;

  private GameObject[] friendList ;
  private GameObject[] temporaire ;
  private GameObject friend ;
  private GameObject gameOverRenardPanel;

  List<GameObject> friendListMinusMe = new List<GameObject>();
  List<GameObject> gotRidList = new List<GameObject>();


    private string tagPrey = "Poule1";
    private string tagPredator = "Vipere1";
    private string tagFriend = "Renard1";


    private Vector3 homePoule = new Vector3( -21.0f,  0.0f, 21.0f);
    private Vector3 homeRenard = new Vector3(- 21.0f,  0.0f, -21.0f);


    void Start()
    {
      float x = Random.Range(-23, 23);
      float  y = 0;
      float z = Random.Range(-23, 23);
      Vector3 depart  = new Vector3(x, y, z);
      transform.position = depart;
      agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
      predatorList = GameObject.FindGameObjectsWithTag(tagPredator);
      preyList = GameObject.FindGameObjectsWithTag(tagPrey);
      friendList = GameObject.FindGameObjectsWithTag(tagFriend);
      getRidOfMyselfInFriendArray(friendList, out friendListMinusMe);
      enChasse = false;
      prisEnChasse = false;
      touched = false;
      firstVictim = false ;
      amiArrete = false ;
      gameOverRenard = false ;
      agent.isStopped = false ;
      gameOver = false ;
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

    bool amiArreteEnVue(out Vector3 cible)
    {
      if(friendListMinusMe.Count != 0)
      {  // test seulement dans le cas ou le renard est seule (test unitaire)

        List<GameObject> friendArrestedVisibleList = new List<GameObject>();
        cible = Vector3.zero;
        unCamaradeALiberer = false ;
        foreach(GameObject camarade in friendListMinusMe)
        {
              MoveRenard controlFriendArrested = camarade.GetComponent<MoveRenard>();
              bool friendArrested = controlFriendArrested.touched; // access this particular touched variable
              // on teste si la prey est a distance de vue  ..................................  et    dans le champ de vision  ...........................................................................    et    s'il n'y a pas une proie plus proche
              if ( ((camarade.transform.position - agent.transform.position).magnitude < sightRange) && (Vector3.Angle(camarade.transform.position - agent.transform.position, agent.transform.forward) < sightAngle) && ( friendArrested == true ) )
                {
                  unCamaradeALiberer = true ;
                  friendArrestedVisibleList.Add(camarade) ;
                 }
          }
          Transform tempResult = getClosest(friendArrestedVisibleList);
          cible = tempResult.position ;
          return unCamaradeALiberer;
      }
      else
      {
        cible = new Vector3 (0.0f,0.0f,0.0f);
        unCamaradeALiberer = false ;
        return unCamaradeALiberer;
      }
    }

    void RunAway(Vector3 point)
    {
        if (AwayPoint(point, range, out Vector3 goal))
        {
           LayerMask mask = LayerMask.GetMask("Wall");
           Vector3 destination = agent.transform.position + goal ; //le vecteur goal est appliqué depuis la position de l'agent
          // This would cast rays only against colliders in layer 8.
          // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
          float distanceRay = Vector3.Distance(transform.position,destination);
          bool hitForward = Physics.Raycast(agent.transform.position, agent.transform.forward, out RaycastHit hitRayForward, distanceRay, mask);
          bool hitLeft = Physics.Raycast(agent.transform.position, -agent.transform.right, out RaycastHit hitRayLeft, distanceRay, mask) ;
          bool hitRight = Physics.Raycast(agent.transform.position, agent.transform.right, out RaycastHit hitRayRight, distanceRay, mask) ;

          if (hitForward)
          {
            if (hitRight)
            {
              if (hitLeft && (hitRayLeft.distance < hitRayRight.distance ))
              {
                destination = goal + agent.transform.position + agent.transform.right * offsetFromWall * 2.0f;
              }
              else
              {
                destination = goal + agent.transform.position - agent.transform.right * offsetFromWall * 2.0f;
              }
            }
            else if (hitLeft)
            {
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
          Vector3 distance = predatorPosition - transform.position;
          Vector3 direction = distance.normalized;
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

      bool StringComparison (string s1, string s2)
      {
          if (s1.Length != s2.Length) return false;
          for (int i = 0; i < s1.Length; i++)
          {
              if (s1[i] != s2[i]) {
                  return false;
              }
          }
          return true;
      }

    void getRidOfMyselfInFriendArray (GameObject[] anyList,out List<GameObject> gotRidList)
    {
      gotRidList = new List<GameObject>();
      for (int i = 0; i < anyList.Length; i++)
        {
          GameObject item = anyList[i];
            if (StringComparison(item.name, gameObject.name) == false  )
            {
              gotRidList.Add(item);
            }
          }
     }

     bool endOfGame()
     {
       for (int j = 0; j < preyList.Length; j++)
         {
           prey = preyList[j] ;
           MovePoule controlTouchedPrey = prey.GetComponent<MovePoule>();
           preyTouched = controlTouchedPrey.touched;
           if(preyTouched == false)
           {
             return false;
           }
         }
         return true;
     }

    bool CibleEnVue(out Vector3 result)
    {
      if(preyList.Length != 0){  // test seulement dans le cas ou le renard est seule (test unitaire)

        List<GameObject> preyVisibleList = new List<GameObject>();
        result = Vector3.zero;
        enChasse = false ;
          for (int i = 0; i < preyList.Length; i++)
          {
              GameObject prey = preyList[i];
              MovePoule controlTouchedPrey = prey.GetComponent<MovePoule>();
              preyTouched = controlTouchedPrey.touched; // access this particular touched variable
              // on teste si la prey est a distance de vue  ..................................  et    dans le champ de vision  ...........................................................................    et    s'il n'y a pas une proie plus proche
              if ( ((prey.transform.position - agent.transform.position).magnitude < sightRange) && (Vector3.Angle(prey.transform.position - agent.transform.position, agent.transform.forward) < sightAngle) && ( preyTouched == false ) )
                {
                  enChasse = true ;
                  preyVisibleList.Add(prey) ;

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

    bool getPlusProcheAmiArrete(out Vector3 result)
    {
        List<GameObject> friendArreteList = new List<GameObject>();
        amiArrete = false ;
          foreach( GameObject friend in friendListMinusMe)
          {
              MoveRenard controlTouchedFriend = friend.GetComponent<MoveRenard>();
              bool friendTouched = controlTouchedFriend.touched; // access this particular touched variable
              // on teste si la prey est a distance de vue  ..................................  et    dans le champ de vision  ...........................................................................    et    s'il n'y a pas une proie plus proche
              if ( friendTouched == true )
                {
                  friendArreteList.Add(friend) ;
                  amiArrete = true ;
                }
          }
          Transform tempResult = getClosest(friendArreteList);
          result = tempResult.position ;
          return amiArrete ;
    }

    Transform getClosest(List<GameObject> GameObjectVisibleList)
    {
      if(GameObjectVisibleList.Count != 0) {
      // initialisation
        float distance = Vector3.Distance(transform.position, GameObjectVisibleList[0].transform.position);
        Transform result = GameObjectVisibleList[0].transform;
        foreach (GameObject preyVisible in GameObjectVisibleList)
        {
            if (Vector3.Distance(transform.position, preyVisible.transform.position) < distance)
            {
              distance = Vector3.Distance(transform.position, preyVisible.transform.position) ;
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

    bool prisPourCible(out Vector3 result)
    {
        for (int i = 0; i < predatorList.Length; i++)
        {
            GameObject predator = predatorList[i];
            MoveVipere controlPredatorTouched = predator.GetComponent<MoveVipere>();
            predatorTouched = controlPredatorTouched.touched; // access this particular touched variable
            if ( ((predator.transform.position - agent.transform.position).magnitude < sightRange) && (predatorTouched == false) )
            {
                Vector3 cibleDir = predator.transform.position - agent.transform.position;
                if (Vector3.Angle(cibleDir, agent.transform.forward) < sightAngle)
                {
                    result = predator.transform.position;
                    prisEnChasse = true;
                    return true;
                }
            }
        }
        result = Vector3.zero;
        return false;
    }

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
            }
          }
        }
      }

    void OnCollisionEnter(Collision collision)
    {
      if (collision.gameObject.tag == tagPrey)
        {
          //agent.SetDestination(new Vector3(0,0,0));
          // Destroy(collision.gameObject);
          MovePoule controlCollisionPrey = collision.gameObject.GetComponent<MovePoule>();
          if (controlCollisionPrey.touched == false)
          {
            controlCollisionPrey.touched = true; // access this particular touched variable
            collision.gameObject.transform.position = homePoule ;
          }
          else
          {
            // print(gameObject.name + " : Get out of my way !!") ;
          }
        }
      else if (collision.gameObject.tag == tagFriend)
          {

            MoveRenard controlCollisiontFriend = collision.gameObject.GetComponent<MoveRenard>();
            if ( controlCollisiontFriend.touched == true && touched == false)
            {
              controlCollisiontFriend.touched = false ;
            }
            else
            {
              // print("Pardon copain");
            }

          }
    }

    // Update is called once per frame
    void Update()
    {
      gameOverRenard = endOfGame();
      amiArrete = getPlusProcheAmiArrete(out Vector3 friendPosition) ; // necessaire pour les comportement des personnes arretés
      enChasse = CibleEnVue(out Vector3 preyPosition);
      prisEnChasse = prisPourCible(out Vector3 predatorPosition);
      unCamaradeALiberer = amiArreteEnVue(out Vector3 friendToBeSavedPosition); // necessaire pour sauver les amis.

      float sliderValueAgr = SliderManagerAnger.sliderAgressiviteRenard.value ;
      float sliderValueAlt = SliderManagerSolidarity.sliderSolidaireRenard.value ;
      float facteurAlt = Mathf.Pow(10, sliderValueAlt) ;
      float facteurAgr = Mathf.Pow(10, sliderValueAgr) ;
      pondAppetit = facteurAgr  * pondPeur ;
      pondAltruist = facteurAlt  * pondEgoist ;

      if(gameOverRenard)
      {
        SliderManagerSolidarity.gameOverRenardPanel.SetActive(true) ;
        Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;
        gameOver = true ;
      }

      if(gameOver)
      {
        agent.isStopped = true ;
      }

      if (touched == true)
      {
        if (amiArrete)
        {
          if (firstVictim)
          {
            agent.SetDestination(homeRenard) ;
          }
          else  // afin qu'il ne se pourchasse pas lui meme
          {
            RunAfter(MoveRenard.premierRenardArrested.transform.position);
          }
        }
        else // n'est lu que pour le premier arreté. on l'immobilise alors, pour ne pas que la grappe sorte de la prison
        {
          premierRenardArrested = gameObject;
          firstVictim = true ;
        }
      }
      else
      {
        if( enChasse == true && prisEnChasse == true )
        {
          if (unCamaradeALiberer == true )
          {
            if  (Vector3.Distance(preyPosition,transform.position)*pondAppetit > Vector3.Distance(predatorPosition,transform.position)*pondPeur)  // mode intermédiaire
            {
              if  (Vector3.Distance(preyPosition,transform.position)*pondEgoist > Vector3.Distance(friendToBeSavedPosition,transform.position)*pondAltruist) // mode intermédiaire
                   { RunAfter(preyPosition); } // mode chasse
              else { RunAfter(friendToBeSavedPosition) ; } // mode solidaire
            }
            else
            {
              if  (Vector3.Distance(predatorPosition,transform.position)*pondEgoist > Vector3.Distance(friendToBeSavedPosition,transform.position)*pondAltruist) // mode intermédiaire
                 { RunAway( predatorPosition) ; } // mode fuite
            else { RunAfter(friendToBeSavedPosition) ; } // mode solidaire
            }
          }
          else
          {
            if  (Vector3.Distance(preyPosition,transform.position)*pondAppetit > Vector3.Distance(predatorPosition,transform.position)*pondPeur) // mode intermédiaire
                 {  RunAfter(preyPosition) ; } // mode chasse
            else {  RunAway( predatorPosition) ; }  // mode fuite
          }
        }
        else if (enChasse == false && prisEnChasse == false)
        {
          if (unCamaradeALiberer)
               {  RunAfter(friendToBeSavedPosition); } // mode solidaire
          else { FreeWalk() ; } // mode balade
        }
        else if (enChasse == true && prisEnChasse == false)
        {
          if  (unCamaradeALiberer)
          {
            if  ( Vector3.Distance(preyPosition,transform.position)*pondEgoist > Vector3.Distance(friendToBeSavedPosition,transform.position)*pondAltruist) // mode intermédiaire
                 { RunAfter(preyPosition); } // mode chasse
            else { RunAfter(friendToBeSavedPosition) ; } // mode solidaire
          }
          else  {  RunAfter( preyPosition) ; }
        }
        else if (enChasse == false && prisEnChasse == true)
        {
          if  (unCamaradeALiberer)
          {
            if( Vector3.Distance(predatorPosition,transform.position)*pondEgoist > Vector3.Distance(friendToBeSavedPosition,transform.position)*pondAltruist) // mode intermédiaire
               { RunAway( predatorPosition) ; } // mode fuite
            else { RunAfter(friendToBeSavedPosition) ; }  // mode solidaire
          }
          else  {  RunAway( predatorPosition) ; }
        }
      }
    }
  }
