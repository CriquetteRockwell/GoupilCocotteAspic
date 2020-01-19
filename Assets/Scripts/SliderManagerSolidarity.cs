using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManagerSolidarity : MonoBehaviour
{
   static public Slider sliderSolidairePoule ;
   static public Slider sliderSolidaireVipere ;
   static public Slider sliderSolidaireRenard ;


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

    sliderSolidairePoule = GameObject.Find("sliderSolidairePoule").GetComponent<Slider>();
    sliderSolidaireVipere = GameObject.Find("sliderSolidaireVipere").GetComponent<Slider>();
    sliderSolidaireRenard = GameObject.Find("sliderSolidaireRenard").GetComponent<Slider>();

 }

public void SliderSolidaire()
{
  float slidValPoul = sliderSolidairePoule.value ;
  float valuePoule = Mathf.Pow(10, slidValPoul) ;
  Debug.Log("solidarité / egoisme Poule = " + valuePoule);

  float slidValVip = sliderSolidaireVipere.value ;
  float valueVipere = Mathf.Pow(10, slidValVip) ;
  Debug.Log("solidarité / egoisme Vipere = " + slidValVip);

  float slidValRen = sliderSolidaireVipere.value ;
  float valueRenard = Mathf.Pow(10, slidValRen) ;
  Debug.Log("solidarité / egoisme Renard = " + slidValRen);

}

   void Update()
   {
   }
}
