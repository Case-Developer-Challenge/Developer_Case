using System.Collections;
using TMPro;
using UIPanels;
using UnityEngine;

namespace Managers
{
    public class UIManager : PersistentSingleton<UIManager>
    {
        public ProductionPanel productionPanel;
        public InformationPanel informationPanel;
        public TMP_Text warningTmp;
        private Coroutine _warningCoroutine;
        public void ShowWarning(string message)
        {
            if (_warningCoroutine != null)
                StopCoroutine(_warningCoroutine);
            warningTmp.text = message;
            _warningCoroutine = StartCoroutine(WarningCoroutine());
        }
        private IEnumerator WarningCoroutine()
        {
            warningTmp.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            warningTmp.gameObject.SetActive(false);
        }
    }
}