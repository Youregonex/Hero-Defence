using Youregone.GameplaySystems;
using UnityEngine;
using Zenject;

namespace Youregone.GameInstallers
{
    public class GameplaySystemsInstaller : MonoInstaller
    {
        [SerializeField] private EnemySpawner _enemySpawner;

        public override void InstallBindings()
        {
            Container
                .Bind<EnemySpawner>()
                .FromInstance(_enemySpawner)
                .AsSingle();

            Container
                .Bind<PauseManager>()
                .AsSingle();

            Container
                .Bind<GameState>()
                .AsSingle();
        }
    }
}
