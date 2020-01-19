using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManagerAnger : MonoBehaviour
{

   static public Slider sliderAgressivite ;

 void Start()
 {
   sliderAgressivite = GameObject.Find("sliderAgressivite").GetComponent<Slider>();
 }

public void SliderPeur()
{
  Debug.Log("Agressivite / peur = " + sliderAgressivite.value);
}

void Update()
{
}

}
