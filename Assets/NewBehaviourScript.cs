using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
#if UNITY_IOS
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void _SaveToGallery(string filePath);
#endif

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveImageToGallery()
    {
        string filename = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + filename;
        Debug.Log("Save Screenshot " + filePath);
        ScreenCapture.CaptureScreenshot(filename);

#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(WaitingToSaveGallery(0.1f, () => {
                string savePath = "/mnt/sdcard/DCIM/Camera/" + filename;
                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                }
                //保存文件
                File.WriteAllBytes(savePath, File.ReadAllBytes(filePath));

                Debug.Log("Save Screenshot 1" + filePath);
                //sendBroadcast(new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE, Uri.fromFile(new File("your path"))););
                AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    Debug.Log("SaveFoodImage 2" + filePath);
                    //Intent scanIntent = new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE);
                    AndroidJavaObject scanIntent = new AndroidJavaObject("android.content.Intent", "android.intent.action.MEDIA_SCANNER_SCAN_FILE");
                    Debug.Log("SaveFoodImage 3" + filePath);
                    //scanIntent.setData(Uri.fromFile(new File(filePath)));
                    scanIntent.Call("setData", new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("parse", savePath));//"file://"+ savePath
                    Debug.Log("SaveFoodImage 4" + filePath);
                    //getActivity().sendBroadcast(scanIntent);
                    currentActivity.Call<AndroidJavaObject>("getActivity").Call("sendBroadcast", scanIntent);
                    Debug.Log("SaveFoodImage 5" + filePath);
                }));
            }));
        }
#elif UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            StartCoroutine(WaitingToSaveGallery(0.1f, () => { _SaveToGallery(filePath); }));
        }
#endif
    }

    IEnumerator WaitingToSaveGallery(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }

    void AndroidToast(string text)
    {
        AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            toastClass.CallStatic<AndroidJavaObject>("makeText", currentActivity, text, toastClass.GetStatic<int>("LENGTH_SHORT")).Call("show");
        }));

        /*
        // 匿名方法中第二个参数是安卓上下文对象，除了用currentActivity，还可用安卓中的GetApplicationContext()获得上下文。
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        */
    }
    
}
