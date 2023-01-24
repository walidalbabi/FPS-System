using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DeathLogScript : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI _killerTextField;
    [SerializeField] TextMeshProUGUI _victimTextField;
    [SerializeField] Image _killImage;

    [SerializeField] private float _timeHealth = 2f;

    public void SetDeathLog(string killerName, string victimName, bool isHeadshot)
    {
        _killerTextField.text = killerName;
        _victimTextField.text = victimName;
        StartCoroutine(DisableAfterDelay(_timeHealth));
    }

    IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
