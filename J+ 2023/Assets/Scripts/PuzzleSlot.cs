using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSlot : MonoBehaviour {
       [SerializeField] private AudioSource _source;
       [SerializeField] private AudioClip _completeClip;

    public void Placed(){
        _source.PlayOneShot(_completeClip);
    }
}
