using Youregone.PlayerComponents;
using UnityEngine;
using Zenject;

namespace Youregone.GameInstallers
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private int _playerMaxHealth;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlayerDamageZone _playerDamageZone;

        public override void InstallBindings()
        {
            Container
                .Bind<PlayerController>()
                .FromInstance(_playerController)
                .AsSingle();

            Container
                .Bind<PlayerHealth>()
                .AsSingle()
                .WithArguments(_playerMaxHealth);

            Container
                .Bind<PlayerDamageZone>()
                .FromInstance(_playerDamageZone)
                .AsSingle();
        }
    }
}
