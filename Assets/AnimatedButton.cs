using UnityEngine;
using System.Collections;
using System;

public class AnimatedButton : MonoBehaviour
{

    public float returnSpeed = 10.0f;
    public float activationDistance = 0.5f;

    protected bool pressed = false;
    protected bool released = false;
    protected Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
      AnimatedButtonUpdate();
    }
    void AnimatedButtonUpdate(){
      released = false;
      float distance;

      Vector3 localPos = transform.localPosition;
      localPos.x = startPosition.x;
      localPos.z = startPosition.z;
      localPos.y = Mathf.Clamp(localPos.y, -0.4f, -0.001f);

      transform.localPosition = localPos;

      // Move back to startPosition if not obstructed
      transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, Time.deltaTime * returnSpeed);

      // trigger distance calculation
      Vector3 xyzdistance = transform.localPosition - startPosition;
      distance = Math.Abs(xyzdistance.y);


      float pressComplete = Mathf.Clamp(1 / activationDistance * distance, 0f, 1f);

      //Activate pressed button
      if (pressComplete >= 0.02f && !pressed && !released)
      {
          pressed = true;
          Debug.Log("pressed " + localPos.y);
      }
      //Dectivate unpressed button
      else if (pressComplete <= 0.005f && pressed)
      {
          pressed = false;
          released = true;
          Debug.Log("released " + localPos.y);
      }
    }
}
