using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [SerializeField]private Camera referenceCamera;
    [SerializeField]private CharacterController controller;
    [SerializeField]private Animator _animator;
    void Awake()
    {
        if (!referenceCamera)
        {
            Debug.LogError("Require referenceCamera",this);
            enabled = false;
            return;
        }
        
        if(!controller)
        {
            Debug.LogError("Require character controller.",this);
            enabled = false;
            return;
        }
        if(!_animator)
        {
            Debug.LogError("Require animator.",this);
            enabled = false;
            return;
        }
    }
    public Camera GetCamera()
    {
        if(!referenceCamera)
        {
            Debug.LogError("Require reference camera.",this);
            enabled = false;
            return null;
        }
        return referenceCamera;
    }
    public CharacterController GetCharacterController()
    {
        if (!controller)
        {
            Debug.LogError("Require character controller.",this);
            enabled = false;
            return null;
        }
        return controller;
    }
    public Animator GetAnimator()
    {
        if (!_animator)
        {
            Debug.LogError("Require animator.",this);
            enabled = false;
            return null;
        }
        return _animator;
    }

}
