using CarLotJam.CameraModule;
using CarLotJam.CarModule;
using CarLotJam.ClickModule;
using CarLotJam.GameManagementModule;
using CarLotJam.LevelModule;
using CarLotJam.UIModule;
using Zenject;

namespace CarLotJam
{
    public class ZenjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<UIManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<LevelManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<LevelSignals>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ClickManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CameraManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CarManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
