using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterProfile", menuName = "ScriptableObjects/CharacterProfile")]
public class CharacterAttackProfile : ScriptableObject
{
    public string CharacterName;
    public List<AttackData> AttackPatterns = new List<AttackData>();
}