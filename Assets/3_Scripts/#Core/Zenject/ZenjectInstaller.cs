using CarLotJam.CameraModule;
using CarLotJam.ClickModule;
using CarLotJam.GameManagementModule;
using CarLotJam.LevelModule;
using Zenject;

namespace CarLotJam
{
    public class ZenjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<LevelManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<LevelSignals>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ClickManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CameraManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
