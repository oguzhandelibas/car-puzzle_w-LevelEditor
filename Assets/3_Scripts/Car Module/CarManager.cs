using System.Collections.Generic;
using UnityEngine;

namespace CarLotJam.CarModule
{
    public class CarManager : AbstractSingleton<CarManager>
    {
        public List<CarController> waitingCars = new List<CarController>();

        public void AddWaitingList(CarController carController)
        {
            waitingCars.Add(carController);
        }

        private void Update()
        {
            if(waitingCars.Count == 0) return;

            for (int i = 0; i < waitingCars.Count; i++)
            {
                if (waitingCars[i].CanMove()) waitingCars.Remove(waitingCars[i]);
            }
        }
    }
}
