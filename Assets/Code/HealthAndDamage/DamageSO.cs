using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Object", menuName = "Damage Objects/Simple Damage")]
public class Damage : ScriptableObject
{
    public float BaseDamage = 30;
    public float PiercingDamage = 10;
    public List<string> TargetsTags = new List<string>() { "default" };
}
