using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirHockeySettings : MonoBehaviour
{
    public static AirHockeySettings instance;

    public float maxDistanceFromPuckForReward = 3.0f;
    public float strikerSpeedThreshold = 3.0f;
    public float hitForce = 4000.0f;
    public float strikerSpeed = 30.0f;

    private void Awake() => instance = this;
}
