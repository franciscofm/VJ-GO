using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Entity { 
    protected override IEnumerator MoveRoutine(Entity entity, Spot spot, float duration, AnimationCurve curve, Action endAction = null)
    {
        anim.Play("moveAnimation");
        return base.MoveRoutine(entity, spot, duration, curve, endAction);
    }
}
