using System;
using UnityEngine;

namespace CarLotJam.StickmanModule
{
    public class StickmanAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        StickmanAnimTypes _stickmanAnimNowSelect = StickmanAnimTypes.IDLE;

        public void PlayAnim(StickmanAnimTypes stickmanAnimName, float blend = 0)
        {
            animator.SetFloat("Blend", blend);
            if (_stickmanAnimNowSelect == stickmanAnimName)
                return;

            foreach (StickmanAnimTypes item in (StickmanAnimTypes[])Enum.GetValues(typeof(StickmanAnimTypes)))
                animator.SetBool(item.ToString(), item == stickmanAnimName);

            _stickmanAnimNowSelect = stickmanAnimName;
        }
    }
}
