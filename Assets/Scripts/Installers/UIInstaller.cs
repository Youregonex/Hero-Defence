using Youregone.UI;
using UnityEngine;
using Zenject;

namespace Youregone.GameInstallers
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private PlayerHealthUI _playerHealthUI;

        public override void InstallBindings()
        {
            Container
                .Bind<PlayerHealthUI>()
                .FromInstance(_playerHealthUI)
                .AsSingle();
        }
    }
}
