using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General
{
    public static class AnimatorExtensionMethods
    {
        public static void Play(this Animator animator, MotionInfo info)
        {
            animator.speed = info.speed;
            animator.Play(info.motion.name);
        }
    }
}
