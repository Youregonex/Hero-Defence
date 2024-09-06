using UnityEngine;
using TMPro;

namespace Youregone.UI
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private TextMeshProUGUI _playerHealthText;

        public void SetHealthUI(int currentHealth)
        {
            _playerHealthText.text = currentHealth.ToString();
        }
    }
}