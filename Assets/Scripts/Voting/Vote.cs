using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vote", menuName = "Scriptable Objects/Vote")]
public class Vote : ScriptableObject
{
    [field:SerializeField] public string VoteDescription { get; private set; }

    [field: SerializeField] public string[] Choices { get; private set; }
    [field: SerializeField] public string[] ButtonTexts { get; private set; }

    [field: SerializeField] public VoteEffectBase[] Effects { get; private set; }
}
