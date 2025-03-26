using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace SignageHADO
{
    public class BluetoothDevice : MonoBehaviourCustom
    {
        [SerializeField] private Button _btn = null;
        [SerializeField] private TextMeshProUGUI _label = null;

        private UnityAction<string> _onClick = null;

        private void Start()
        {
            _btn.onClick.AddListener(OnClick);
        }

        public void Set(string label, UnityAction<string> onClick)
        {
            gameObject.name = label;

            if (_label.text != label)
                _label.text = label;

            _onClick = onClick;
        }

        private void OnClick()
        {
            if (_onClick != null)
                _onClick(_label.text);
        }
    }
}
