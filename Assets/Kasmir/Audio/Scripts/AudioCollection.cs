using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioCollection", menuName = "Extra/AudioCollection", order = 1)]
[System.Serializable]
public class AudioCollection : ScriptableObject {
  [SerializeField]
  public List<AudioClip> audioClips;
}
