using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Bullet : MonoBehaviour
{
    [SerializeField] private Image _bulletImage;

    private Image _bgBulletImage;

    private void Awake()
    {
        _bgBulletImage = GetComponent<Image>();
    }

    public void SetBullet(Sprite BulletImg)
    {
        _bulletImage.sprite = BulletImg;
        _bgBulletImage.sprite = BulletImg;
      //  _bulletImage.SetNativeSize();
    }

    public void FullBullet()
    {
        _bulletImage.enabled = true;
    }

    public void EmptyBullet()
    {
        _bulletImage.enabled = false;
    }
}
