using UnityEngine;
using UnityEngine.UI;

 public class TimeButton : MonoBehaviour

 {

 void Start()
  {
     }

 public void TogglePause() {
         Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;
     }

public void Accelerate()
  {
    Time.timeScale = Time.timeScale * 5.0f ;
  }

  public void SlowDown()
    {
      Time.timeScale = Time.timeScale / 5.0f ;
    }
    public void afficherGagnant()
    {
      if(MovePoule.gameOverPoule)
      {

      }
      else if (MoveRenard.gameOverRenard)
      {

      }
      else if (MoveVipere.gameOverVipere)
      {

      }
    }

 }
