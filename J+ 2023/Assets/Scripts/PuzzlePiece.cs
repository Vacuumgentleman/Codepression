using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour {

    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _pickupClip, _dropClip;

    private bool _dragging;
    private Vector2 _offset;
    private Vector2 _originalPosition;

    void Awake() {
        // Almacena la posición original en Awake, solo se ejecuta una vez.
        _originalPosition = transform.position;
    }

    void Update() {
        if (!_dragging) return;
        var mousePosition = GetMousePos();
        transform.position = mousePosition - _offset;
    }

    void OnMouseDown() {
        _dragging = true;
        _source.PlayOneShot(_pickupClip);

        // Almacena la posición original al comienzo del arrastre.
        _originalPosition = transform.position;

        _offset = GetMousePos() - (Vector2)transform.position;
    }

    void OnMouseUp() {
        // Restaura la posición original al soltar el objeto.
        transform.position = _originalPosition;
        _dragging = false;
        _source.PlayOneShot(_dropClip);
    }

    Vector2 GetMousePos() {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
