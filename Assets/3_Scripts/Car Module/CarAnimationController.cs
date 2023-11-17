using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace CarLotJam.CarModule
{
    public class CarAnimationController : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private CarController carController;
        [SerializeField] private Transform carBody;
        [SerializeField] private Transform leftDoor;
        [SerializeField] private Transform rightDoor;

        #endregion

        #region CAR ANIMATION CONTROL

        public void FindNearestDoor(Vector3 stickmanPos)
        {
            float distance = Vector3.Distance(leftDoor.position, stickmanPos);
            if (Vector3.Distance(rightDoor.position, stickmanPos) < distance) PlayAnim(CarAnimType.DOOR_OPENING, -1);
            else PlayAnim(CarAnimType.DOOR_OPENING, 1);
        }

        public void PlayAnim(CarAnimType carAnimType, int multiplier)
        {
            switch (carAnimType)
            {
                case CarAnimType.DOOR_OPENING:
                    OpenDoor(multiplier);
                    break;
                case CarAnimType.DOOR_CLOSING:
                    CloseDoor(multiplier);
                    break;
                case CarAnimType.MOVE:
                    MoveAcceleration(multiplier);
                    break;
            }
        }

        public void MoveAcceleration(int multiplier)
        {
            carBody.DOLocalRotate(new Vector3(multiplier * 7, 0, 0), 0.2f).SetEase(Ease.InOutBack)
                .OnComplete((() => carBody.DOLocalRotate(new Vector3(0, 0, 0), .4f).SetEase(Ease.InBounce)));
        }
        private void OpenDoor(int multiplier)
        {
            Transform door = multiplier == 1 ? leftDoor : rightDoor;
            door.DOLocalRotate(new Vector3(0, 110 * multiplier, 0), 0.5f).SetEase(Ease.OutBounce)
                .OnComplete((() => CloseDoor(multiplier)));
        }
        private async Task CloseDoor(int multiplier)
        {
            carBody.DOLocalRotate(new Vector3(0, 0, multiplier * 12), 0.35f).OnComplete(
                (() => carBody.DOLocalRotate(new Vector3(carBody.transform.rotation.x, 0, multiplier * 5), 0.15f)
                    .OnComplete(((delegate
                    {
                        carBody.DOLocalRotate(new Vector3(carBody.transform.rotation.x, 0, 0), 0.15f);
                    })))));

            Transform door = multiplier == 1 ? leftDoor : rightDoor;
            await Task.Delay(500);
            door.DOLocalRotate(new Vector3(0, 0, 0), 0.30f);
        }

        #endregion
    }
}
