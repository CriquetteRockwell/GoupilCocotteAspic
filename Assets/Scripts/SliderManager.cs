using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
   int n;
   static public Slider sliderSolidaire ;
   static public Slider sliderAgressivite ;

 void Start()
 {
   sliderSolidaire = GameObject.Find("sliderSolidaire").GetComponent<Slider>();
   sliderAgressivite = GameObject.Find("sliderAgressivite").GetComponent<Slider>();
 }

public void SliderPeur()
{
  Debug.Log("value = " + sliderAgressivite.value);
}

public void SliderSolidaire()
{
  Debug.Log("value = " + sliderSolidaire.value);

}

   void Update()
   {
   }
}
