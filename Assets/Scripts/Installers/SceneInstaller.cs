using UnityEngine;
using Zenject;
using Youregone.GameplaySystems;
using Youregone.PlayerComponents;

namespace Youregone.SceneInstaller
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private PlayerController _playerController;

        public override void InstallBindings()
        {
            Container
                .Bind<EnemySpawner>()
                .FromInstance(_enemySpawner)
                .AsSingle();

            Container
                .Bind<PlayerController>()
                .FromInstance(_playerController)
                .AsSingle();
        }
    }
}
