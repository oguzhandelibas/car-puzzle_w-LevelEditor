using System.Collections;
using System.Collections.Generic;
using CarLotJam.GridModule;
using CarLotJam.StickmanModule;
using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam.ClickModule
{
    public class ClickManager : MonoBehaviour
    {
        private StickmanController _stickmanController;

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
                        if (iClickable.IsGround())
                        {
                            if (_stickmanController && _stickmanController.IsHold)
                            {
                                Ground ground = hit.transform.GetComponent<Ground>();
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
                        else
                        {
                            if (_stickmanController)
                            {
                                _stickmanController.IsHold = false;
                            }
                            _stickmanController = hit.transform.GetComponent<StickmanController>();
                            _stickmanController.IsHold = true;
                            iClickable.OnClick();
                        }
                    }
                }
            }
        }
    }
}
