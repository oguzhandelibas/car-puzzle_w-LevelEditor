using System.Collections;
using System.Collections.Generic;
using CarLotJam.CarModule;
using CarLotJam.GridModule;
using CarLotJam.StickmanModule;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam.ClickModule
{
    public class ClickManager : MonoBehaviour
    {
        private StickmanController _stickmanController;
        private CarController _carController;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) // Sol týklama
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.TryGetComponent(out IClickable iClickable))
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
                        
                        if (_stickmanController) _stickmanController.IsHold = false;
                        if (_carController) _carController.Release();
                        
                        if (hit.transform.TryGetComponent(out CarController carController))
                        {
                            _carController = carController;
                            _carController.Hold();
                        }

                        if (hit.transform.TryGetComponent(out StickmanController stickmanController))
                        {
                            _stickmanController = stickmanController;
                            _stickmanController.IsHold = true;
                        }

                        

                        iClickable.OnClick();

                        
                    }
                }
            }
        }
    }
}
