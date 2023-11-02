using System.Collections;
using System.Collections.Generic;
using CarLotJam.GridModule;
using CarLotJam.StickmanModule;
using UnityEngine;

namespace CarLotJam.ClickModule
{
    public class ClickManager : MonoBehaviour
    {
        private StickmanController _stickmanController;
        private Ground _ground;

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
                            if(_stickmanController && _stickmanController.IsHold) 
                                _stickmanController.SetTargetPoint(iClickable.OnClick());
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
