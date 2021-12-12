using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleViewBase : MonoBehaviour
{
    private const string UrlAssetBundleSprites =
    "https://drive.google.com/uc?export=download&id=1rbUZAwaskfQeWB9MLUyR9BlaQqTUL640";

    private const string UrlAssetBundleAudio =
    "https://drive.google.com/uc?export=download&id=1lmdbA9WjaxzHOcsfp9QHgIV0zPsgGAeD";

    [SerializeField] private DataSpriteBundle[] _dataSpriteBundle;
    [SerializeField] private DataAudioBundle[] _dataAudioBundle;

    private AssetBundle _spriteAssetBundle;
    private AssetBundle _audioAssetBundle;

    protected IEnumerator DownloadAndSetAssetBundle()
    {
        yield return GetSpritesAssetBundle();
        yield return GetAudioAssetBundle();

        if(_spriteAssetBundle == null)
        {
            Debug.LogError("NullAssetsBundle");
            yield break;
        }

        SetDownloadAssets();
        yield return null;
    }

    private IEnumerator GetSpritesAssetBundle()
    {
        var request = UnityWebRequestAssetBundle.GetAssetBundle(UrlAssetBundleSprites);

        yield return request.SendWebRequest();

        while(!request.isDone)
        {
            yield return null;
        }

        StateRequest(request, ref _spriteAssetBundle);
    }

    private IEnumerator GetAudioAssetBundle()
    {
        var request = UnityWebRequestAssetBundle.GetAssetBundle(UrlAssetBundleAudio);

        yield return request.SendWebRequest();

        while (!request.isDone)
        {
            yield return null;
        }

        StateRequest(request, ref _audioAssetBundle);
    }

    private void StateRequest(UnityWebRequest request, ref AssetBundle assetBundle)
    {
        if(request.error == null)
        {
            assetBundle = DownloadHandlerAssetBundle.GetContent(request);
            Debug.Log("AssetBundle Complete");
        }
        else
        {
            Debug.Log("AssetBundle Error");
        }
    }

    private void SetDownloadAssets()
    {
        foreach (var data in _dataSpriteBundle)
        {
            data.AssetBundleImage.sprite = _spriteAssetBundle.LoadAsset<Sprite>(data.AssetBundleName);
        }
        
        foreach (var data in _dataAudioBundle)
        {
            data.AssetBundleAudio.clip = _audioAssetBundle.LoadAsset<AudioClip>(data.AssetBunleName);
            data.AssetBundleAudio.Play();
        }
    }
}
