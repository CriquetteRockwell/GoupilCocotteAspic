using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManagerAnger : MonoBehaviour
{

   static public Slider sliderAgressivitePoule ;
   static public Slider sliderAgressiviteVipere ;
   static public Slider sliderAgressiviteRenard ;


 void Start()
 {
   sliderAgressivitePoule = GameObject.Find("sliderAgressivitePoule").GetComponent<Slider>();
   sliderAgressiviteRenard = GameObject.Find("sliderAgressiviteRenard").GetComponent<Slider>();
   sliderAgressiviteVipere = GameObject.Find("sliderAgressiviteVipere").GetComponent<Slider>();
 }

public void SliderPeur()
{
  float slidValPoul = sliderAgressivitePoule.value ;
  float valuePoule = Mathf.Pow(10, slidValPoul) ;
  Debug.Log("Agressivite / peur Poule = " + sliderAgressivitePoule.value);

  float slidValVip = sliderAgressiviteRenard.value ;
  float valueVipere = Mathf.Pow(10, slidValVip) ;
  Debug.Log("Agressivite / peur Renard = " + sliderAgressiviteRenard.value);

  float slidValRen = sliderAgressiviteVipere.value ;
  float valueRenard = Mathf.Pow(10, slidValRen) ;
  Debug.Log("Agressivite / peur Vipere = " + sliderAgressiviteVipere.value);
}

void Update()
{
}

}
