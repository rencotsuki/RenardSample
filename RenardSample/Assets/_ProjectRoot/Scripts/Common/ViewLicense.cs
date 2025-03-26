using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Renard;

namespace SignageHADO
{
    public class ViewLicense : MonoBehaviourCustom
    {
        [SerializeField] private Image imgFrame = null;
        [SerializeField] private TMP_Text txtExpiryDate = null;
        [SerializeField] private TMP_Text txtLimitDays = null;
        [SerializeField] private Color colorActive = Color.white;
        [SerializeField] private Color colorCaution = Color.yellow;
        [SerializeField] private Color colorWarning = Color.red;

        private DateTime expiryDateTime => LicenseHandler.ExpiryDateTime;
        private string expiryDate => LicenseHandler.ExpiryDate;

        private int limitDayCaution = 0;
        private int limitDayWarning = 0;
        private TimeSpan _timeSpan = default;
        private int _diffDays = 0;

        private void Awake()
        {
            var config = LauncherConfigAsset.Load();
            limitDayCaution = config != null ? config.LimitDayCaution : 0;
            limitDayWarning = config != null ? config.LimitDayWarning : 0;
        }

        private void Start()
        {
            _timeSpan = expiryDateTime - LicenseHandler.GetNow();
            _diffDays = _timeSpan.Days;
            UpdateView(_diffDays);
        }

        private void Update()
        {
            _timeSpan = expiryDateTime - LicenseHandler.GetNow();
            if (_diffDays == _timeSpan.Days)
                return;

            _diffDays = _timeSpan.Days;
            UpdateView(_diffDays);
        }

        private void UpdateView(int diffDays)
        {
            if (txtExpiryDate != null && txtExpiryDate.text != expiryDate)
                txtExpiryDate.text = expiryDate;

            if (txtLimitDays != null)
            {
                if (txtLimitDays.gameObject.activeSelf != IsVisibleLimitDays(diffDays))
                    txtLimitDays.gameObject.SetActive(IsVisibleLimitDays(diffDays));

                if (txtLimitDays.text != $"{(diffDays >= GetYearDays() ? "1 year or more." : $"{diffDays}days")}")
                    txtLimitDays.text = $"{(diffDays >= 365 ? "1 year or more." : $"{diffDays}days")}";
            }

            if (imgFrame != null && imgFrame.color != GetColorFrame(diffDays))
                imgFrame.color = GetColorFrame(diffDays);
        }

        private int GetYearDays()
            => DateTime.IsLeapYear(LicenseHandler.GetNow().Year) ? 366 : 365;

        private bool IsVisibleLimitDays(int limitDay)
        {
            if (limitDayWarning > 0 && limitDay <= limitDayWarning)
                return true;

            if (limitDayCaution > 0 && limitDay <= limitDayCaution)
                return true;

            return false;
        }

        private Color GetColorFrame(int limitDay)
        {
            if (limitDayWarning > 0 && limitDay <= limitDayWarning)
                return colorWarning;

            if (limitDayCaution > 0 && limitDay <= limitDayCaution)
                return colorCaution;

            return colorActive;
        }
    }
}
