using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusEffect
{
    void ChangeMovespeedStatus(float percentage);

    void SetExactMovespeed(float speed);

    void ResetMoveSpeed();
    void StunStatus(float duration);
}
