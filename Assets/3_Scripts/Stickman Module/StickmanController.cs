using ODProjects.LevelEditor;
using UnityEngine;

namespace CarLotJam.StickmanModule
{
    public class StickmanController : MonoBehaviour
    {
        [SerializeField] private SelectedColor stickmanColor;
        [SerializeField] private ColorData colorData;
        [SerializeField] private ColorSetter colorSetter;

        void Start()
        {
            colorSetter.SetMeshMaterials(colorData.Colors[stickmanColor]);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
