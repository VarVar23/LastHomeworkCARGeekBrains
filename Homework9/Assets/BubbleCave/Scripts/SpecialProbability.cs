using System;
using UnityEngine;

[Serializable]
public class SpecialProbability {

    public BallType ballType;

    [Range(0, 10f)]
    public float probability;

}
