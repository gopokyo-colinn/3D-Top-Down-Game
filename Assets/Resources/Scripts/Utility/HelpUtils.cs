using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class HelpUtils 
{
    public static bool CheckAheadForColi(Transform _transform, float _distance)
    {
        return Physics.Raycast(_transform.position + new Vector3(0, 0.3f, 0), _transform.forward, _distance);
    }
    public static bool CheckAheadForColi(Transform _transform, float _distance, string _layerName)
    {
        return Physics.Raycast(_transform.position + new Vector3(0, 0.5f, 0), _transform.forward, _distance, LayerMask.GetMask(_layerName));
    }
    public static bool Grounded(Transform _transform, float _distanceToGround)
    {
        return Physics.Raycast(_transform.position + new Vector3(0,0.2f,0), Vector3.down, _distanceToGround);
    }
    public static void RotateTowards(Transform _transform, Vector3 _target, float _rotationSpeed)
    {
        //_target.y = 0;
        Vector3 _directionToPlayer = (_target - _transform.position).normalized;
        _directionToPlayer.y = 0;
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, Quaternion.LookRotation(_directionToPlayer), _rotationSpeed * Time.fixedDeltaTime);
    }
    public static IEnumerator RotateTowardsTarget(Transform _transform, Transform _target)
    {
        var targetRotation = Quaternion.LookRotation(_target.position - _transform.position);
        Vector3 _targetDir = (_target.transform.position - _transform.position).normalized;
        float dot = -1;

        while (dot <= 0.99f)
        {
            Debug.Log("I rand ");
            dot = Vector3.Dot(_transform.forward, _targetDir);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, 0.2f);
            _transform.localEulerAngles = new Vector3(0, _transform.localEulerAngles.y, 0);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
    }
    public static IEnumerator RotateTowardsTarget(Transform _transform, Quaternion _targetRotation)
    {
        float _fRotateTime = 0;

        while (_fRotateTime < 1f)
        {
            Debug.Log("loop running");
            _fRotateTime += Time.deltaTime;
            _transform.rotation = Quaternion.Slerp(_transform.rotation, _targetRotation, 0.1f);
            _transform.localEulerAngles = new Vector3(0, _transform.localEulerAngles.y, 0);
            yield return null;
        }

        yield return new WaitForSeconds(1);
    }

    public static Vector3 VectorZeroWithY(Rigidbody _rb)
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
