using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave" , menuName = "Waves/Wave")]
public class WaveSO : ScriptableObject
{
    public List<SubWaveSO> subWaves = new List<SubWaveSO>();
}
