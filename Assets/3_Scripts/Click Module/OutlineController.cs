using System;
using System.Threading.Tasks;
using EPOOutline;
using UnityEngine;

namespace CarLotJam.ClickModule
{
    public class OutlineController : MonoBehaviour
    {
        [SerializeField] private Outlinable outlinable;

        private void Start()
        {
            SetOutline(OutlineColorType.NONE);
        }

        public void SetOutline(OutlineColorType outlineColorType)
        {
            outlinable.OutlineParameters.Enabled = true;
            switch (outlineColorType)
            {
                case OutlineColorType.NONE:
                    outlinable.OutlineParameters.Enabled = false;
                    break;
                case OutlineColorType.YELLOW:
                    outlinable.OutlineParameters.Color = Color.yellow;
                    break;
                case OutlineColorType.GREEN:
                    OutlineTimer(Color.green);
                    break;
                case OutlineColorType.RED:
                    OutlineTimer(Color.red);
                    break;
            }
        }

        private async Task OutlineTimer(Color color)
        {
            outlinable.OutlineParameters.Color = color;
            await Task.Delay(1500);
            SetOutline(OutlineColorType.NONE);
        }
    }
}
