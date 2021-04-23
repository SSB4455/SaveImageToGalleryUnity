using System.IO;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
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
        string filename = "Cooking_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + filename;
        Debug.Log("SaveFoodImage " + filePath);
        ScreenCapture.CaptureScreenshot(filename);
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            string savePath = "/mnt/sdcard/DCIM/Camera/" + filename;
            
            {
                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                }
                //保存文件
                File.WriteAllBytes(savePath, File.ReadAllBytes(filePath));

                Debug.Log("SaveFoodImage 1" + filePath);
                //sendBroadcast(new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE, Uri.fromFile(new File("your path"))););
                AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    Debug.Log("SaveFoodImage 2" + filePath);
                    //Intent scanIntent = new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE);
                    AndroidJavaObject scanIntent = new AndroidJavaObject("android.content.Intent", new AndroidJavaClass("android.content.Intent").GetStatic<string>("ACTION_MEDIA_SCANNER_SCAN_FILE"));
                    Debug.Log("SaveFoodImage 3" + filePath);
                    //scanIntent.setData(Uri.fromFile(new File(filePath)));
                    scanIntent.Call("setData", Path.GetDirectoryName(savePath));
                    Debug.Log("SaveFoodImage 4" + filePath);
                    //getActivity().sendBroadcast(scanIntent);
                    currentActivity.Call<AndroidJavaObject>("getActivity").Call("sendBroadcast", scanIntent);
                    Debug.Log("SaveFoodImage 5" + filePath);
                }));
            }
        }
#elif UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            _SaveToGallery(filePath);
        }
#endif
    }

#if UNITY_IOS
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void _SaveToGallery(string filePath);
#endif
}
