using CarLotJam.CarModule;
using CarLotJam.GameManagementModule;
using CarLotJam.GridModule;
using CarLotJam.StickmanModule;
using ODProjects.LevelEditor;
using UnityEngine;
using Zenject;

namespace CarLotJam.ClickModule
{
    public class ClickManager : MonoBehaviour
    {
        #region FIELDS

        [Inject] private TutorialManager _tutorialManager;
        [Inject] private GameManager _gameManager;
        private StickmanController _stickmanController;
        private CarController _carController;

        #endregion

        #region UNITY FUNCTIONS

        private void Update()
        {
            if(!_gameManager.GameHasContinue) return;
            if (Input.GetMouseButtonDown(0))
            {
                ClickableControl();
            }
        }

        #endregion

        #region CLICK CONTROL

        private void ClickableControl()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.TryGetComponent(out IClickable iClickable))
                {

                    GroundCheck(hit, iClickable);

                    if (_stickmanController) { _stickmanController.IsHold = false; }
                    if (_carController) _carController.Release();

                    StickmanCheck(hit);
                    CarCheck(hit, iClickable);

                    iClickable.OnClick();
                }
            }
        }

        private void GroundCheck(RaycastHit hit, IClickable iClickable)
        {
            if (hit.transform.TryGetComponent(out Ground ground))
            {
                if (_stickmanController && _stickmanController.IsHold)
                {
                    if (_stickmanController.SetTargetPoint(iClickable.OnClick()))
                    {
                        ground.SetColorAnim(SelectedColor.Green);
                        _stickmanController.SetEmotion(SelectedEmotion.COOL);
                    }
                    else
                    {
                        ground.SetColorAnim(SelectedColor.Red);
                        _stickmanController.IsHold = false;
                        _stickmanController.SetEmotion(SelectedEmotion.ANGRY);
                    }
                }
            }
        }

        private void StickmanCheck(RaycastHit hit)
        {
            if (hit.transform.TryGetComponent(out StickmanController stickmanController))
            {
                if (!_tutorialManager.tutorialDone)
                {
                    if (_tutorialManager.stickmanController.selectedColor != stickmanController.selectedColor)
                    {
                        return;
                    }
                    else
                    {
                        _tutorialManager.FirstClick();
                    }
                }

                _stickmanController = stickmanController;
                _stickmanController.IsHold = true;

            }
        }

        private void CarCheck(RaycastHit hit, IClickable iClickable)
        {
            if (hit.transform.TryGetComponent(out CarController carController))
            {
                if (_stickmanController)
                {
                    if (!_tutorialManager.tutorialDone)
                    {
                        if (_tutorialManager.carController.selectedColor != carController.selectedColor)
                        {
                            return;
                        }
                        else
                        {
                            _tutorialManager.SecondClick();
                        }
                    }

                    if (carController.selectedColor == _stickmanController.selectedColor)
                    {
                        _stickmanController.FindBestCarPosition(iClickable.PointsList(), carController);
                    }
                    else
                    {
                        _stickmanController.IsHold = false;
                        _stickmanController.SetEmotion(SelectedEmotion.ANGRY);
                    }
                }
                _carController = carController;
                _carController.Hold();
            }
        }

        #endregion
    }
}
