using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionRecoil : RecoilBehaviour
{

    private PlayerInventoryHandler _playerInventory;
    private PlayerMovements _playerMovements;

    private void Awake()
    {
        PlayerSpawner.OnPlayerSpawned += OnPlayerSpawned;
        FirearmShootCompoment.OnFire += OnFireRecoil;
    }


    private void OnDestroy()
    {
        PlayerSpawner.OnPlayerSpawned -= OnPlayerSpawned;
        FirearmShootCompoment.OnFire -= OnFireRecoil;
    }

    private void Update()
    {
        OnUpdate();
    }

    private void OnPlayerSpawned(GameObject objPlayer)
    {
        _playerInventory = objPlayer.GetComponent<PlayerInventoryHandler>();
        _playerMovements = objPlayer.GetComponent<PlayerMovements>();

    }


    public override void OnFireRecoil(WeaponBehaviour fireArm, NetworkOwnership networkOwnership)
    {
        if (fireArm == null) return;
        if (_playerInventory == null) return;

        if (fireArm == _playerInventory.currentSelectedPlayerItem)
            targetRotation += new Vector3(recoil.x, Random.Range(-recoil.y, recoil.y), Random.Range(-recoil.z, recoil.z));

        _playerMovements.pithWithRecoil += recoil.x;
    
    }

    public override void OnUpdate()
    {
        if (_playerMovements == null) return;

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappines * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
        _playerMovements.pithWithRecoil = Mathf.Lerp(_playerMovements.pithWithRecoil, 0, returnSpeed * Time.deltaTime);
    }


}
