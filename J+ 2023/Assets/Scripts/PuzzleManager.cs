using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour {
     [SerializeField] private List<PuzzleSlot> _slotPrefabs;
     [SerializeField] private PuzzlePiece _piecePrefab;
     [SerializeField] private Transform _slotParent, _pieceParent;
     
     void Spawn() {
        // Tu código de inicialización aquí
     }
}
