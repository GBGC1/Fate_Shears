using TMPro;
using UnityEngine;

namespace Script.Manager.Events
{
    public class ChangeWeaponEvent : MonoBehaviour
    {
        private TextMeshProUGUI text;

        void Awake()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        void Start()
        {
            EventBus.Instance().Subscribe<ChangeWeaponEventData>(ChangeForm);
        }

        private void ChangeForm(ChangeWeaponEventData @event)
        {
            text.text = @event.Type;
        }
    }
}