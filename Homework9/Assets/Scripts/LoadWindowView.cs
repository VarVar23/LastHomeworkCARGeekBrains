using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using UnityEngine.UI;

public class LoadWindowView : AssetBundleViewBase
{
    [SerializeField] private Button _loadAssetBundleButton;
    [SerializeField] private AssetReference _loadPrefab;
    [SerializeField] private RectTransform _mountSpawnTransform;
    [SerializeField] private Button _spawnAssetButton;

    private List<AsyncOperationHandle<GameObject>> _addresablePrefabs =
        new List<AsyncOperationHandle<GameObject>>();

    private void Start()
    {
        _loadAssetBundleButton.onClick.AddListener(LoadAsset);
        _spawnAssetButton.onClick.AddListener(CreateAddressablesPrefab);
    }

    private void OnDestroy()
    {
        _loadAssetBundleButton.onClick.RemoveAllListeners();

        foreach(var adressablePrefabs in _addresablePrefabs)
        {
            Addressables.ReleaseInstance(adressablePrefabs);
        }

        _addresablePrefabs.Clear();
    }

    private void LoadAsset()
    {
        _loadAssetBundleButton.interactable = false;

        StartCoroutine(DownloadAndSetAssetBundle());
    }

    private void CreateAddressablesPrefab()
    {
        var addressablePrefab = Addressables.InstantiateAsync(_loadPrefab, _mountSpawnTransform);
        _addresablePrefabs.Add(addressablePrefab);
    }

}
