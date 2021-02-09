using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class HelpUtils 
{
    public static bool CheckAheadForColi(Transform _transform, float _distance)
    {
        return Physics.Raycast(_transform.position + new Vector3(0, 0.5f, 0), _transform.forward, _distance);
    }
    public static bool CheckAheadForColi(Transform _transform, float _distance, string _layerName)
    {
        return Physics.Raycast(_transform.position + new Vector3(0, 0.5f, 0), _transform.forward, _distance, LayerMask.GetMask(_layerName));
    }
    public static bool Grounded(Transform _transform, float _distanceToGround)
    {
        return Physics.Raycast(_transform.position + new Vector3(0,0.2f,0), Vector3.down, _distanceToGround);
    }
    public static void RotateTowardsTarget(Transform _transform, Vector3 _target, float _rotationSpeed)
    {
        Vector3 _directionToPlayer = (_target - _transform.position).normalized;
        _directionToPlayer.y = 0;
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, Quaternion.LookRotation(_directionToPlayer), _rotationSpeed * Time.fixedDeltaTime);
    }
    public static Vector3 VectorZero(Rigidbody _rb)
    {
        return new Vector3(0, _rb.velocity.y, 0);
    }
    public static IEnumerator ChangeBoolAfter(System.Action<bool> _callBack, bool _setBool, float _time)
    {
        yield return new WaitForSeconds(_time);
        _callBack(_setBool);
        //StopAllCoroutines();
    }
    public static IEnumerator WaitForSeconds(System.Action _callBack, float _time)
    {
        yield return new WaitForSeconds(_time);
        _callBack();
    }
}
