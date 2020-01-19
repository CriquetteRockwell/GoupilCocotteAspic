using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManagerSolidarity : MonoBehaviour
{
   static public Slider sliderSolidaire ;

   static public GameObject gameOverRenardPanel;
   static public GameObject gameOverViperePanel;
   static public GameObject gameOverPoulePanel;



 void Start()
  {
    gameOverRenardPanel = GameObject.FindWithTag("GameOverRenard") ;
    gameOverViperePanel = GameObject.FindWithTag("GameOverVipere");
    gameOverPoulePanel = GameObject.FindWithTag("GameOverPoule") ;
    gameOverRenardPanel.SetActive(false) ;
    gameOverViperePanel.SetActive(false) ;
    gameOverPoulePanel.SetActive(false) ;

    sliderSolidaire = GameObject.Find("sliderSolidaire").GetComponent<Slider>();
 }

public void SliderSolidaire()
{
  Debug.Log("solidarité / egoisme = " + sliderSolidaire.value);

}

   void Update()
   {
   }
}
