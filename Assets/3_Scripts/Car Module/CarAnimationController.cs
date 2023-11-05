using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace CarLotJam.CarModule
{
    public class CarAnimationController : MonoBehaviour
    {
        [SerializeField] private Transform carBody;
        [SerializeField] private Transform leftDoor;
        [SerializeField] private Transform rightDoor;

        /*
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            PlayAnim(CarAnimType.RIGHT_DOOR_OPENING);
            yield return new WaitForSeconds(1);
            PlayAnim(CarAnimType.RIGHT_DOOR_CLOSING);
        }*/

        public void FindNearestDoor(Vector3 stickmanPos)
        {
            float distance = Vector3.Distance(leftDoor.position, stickmanPos);
            if(Vector3.Distance(rightDoor.position, stickmanPos) < distance) PlayAnim(CarAnimType.RIGHT_DOOR_OPENING);
            else PlayAnim(CarAnimType.LEFT_DOOR_OPENING);
        }

        public void PlayAnim(CarAnimType carAnimType)
        {
            switch (carAnimType)
            {
                case CarAnimType.LEFT_DOOR_OPENING:
                    OpenLeftDoor();
                    break;
                case CarAnimType.LEFT_DOOR_CLOSING:
                    CloseLeftDoor();
                    break;
                case CarAnimType.RIGHT_DOOR_OPENING:
                    OpenRightDoor();
                    break;
                case CarAnimType.RIGHT_DOOR_CLOSING:
                    CloseRightDoor();
                    break;
                case CarAnimType.MOVE:
                    MoveAcceleration();
                    break;
            }
        }

        private void MoveAcceleration()
        {
            carBody.DOLocalRotate(new Vector3(-15, 0, 0), 0.15f, RotateMode.FastBeyond360).SetEase(Ease.InBack)
                .OnComplete((() => carBody.DOLocalRotate(new Vector3(0, 0, 0), 1.5f, RotateMode.FastBeyond360)));
        }
        private void OpenLeftDoor()
        {
            print("devams");
            leftDoor.DOLocalRotate(new Vector3(0, 110, 0), 0.5f, RotateMode.Fast).SetEase(Ease.OutBounce)
                .OnComplete((() => CloseLeftDoor()));
        }
        private async Task CloseLeftDoor()
        {
            carBody.DOLocalRotate(new Vector3(0, 0, 12), 0.25f, RotateMode.Fast).OnComplete(
                (() => carBody.DOLocalRotate(new Vector3(0, 0, -5), 0.25f, RotateMode.Fast)
                    .OnComplete((() => carBody.DOLocalRotate(new Vector3(0, 0, 0), 0.25f, RotateMode.Fast)))));

            await Task.Delay(500);
            leftDoor.DOLocalRotate(new Vector3(0, 0, 0), 0.30f, RotateMode.Fast);
        }
        private void OpenRightDoor()
        {
            rightDoor.DOLocalRotate(new Vector3(0, -110, 0), 0.5f, RotateMode.Fast).SetEase(Ease.OutBounce)
                .OnComplete((() => CloseRightDoor()));
        }
        private async Task CloseRightDoor()
        {
            carBody.DOLocalRotate(new Vector3(0, 0, -12), 0.25f, RotateMode.Fast).OnComplete(
                (() => carBody.DOLocalRotate(new Vector3(0, 0, 5), 0.25f, RotateMode.Fast)
                    .OnComplete((() => carBody.DOLocalRotate(new Vector3(0, 0, 0), 0.25f, RotateMode.Fast)))));

            await Task.Delay(500);
            rightDoor.DOLocalRotate(new Vector3(0, 0, 0), 0.20f, RotateMode.Fast);
        }
    }
}
