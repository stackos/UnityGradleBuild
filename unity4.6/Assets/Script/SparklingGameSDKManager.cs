using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;



public enum Language
{
	AUTO = 0,
	ENGLISH = 1,
	CHINESE = 2
}

public class SparklingGameSDKManager
{
	#if UNITY_IOS
	[DllImport("__Internal")]    
	private static extern void notification(string scene); 

	[DllImport("__Internal")]    
	private static extern void login1(string scent); 

	[DllImport("__Internal")]    
	private static extern void logout(); 

	[DllImport("__Internal")]    
	private static extern void payClick(string scene,string payment); 

	[DllImport("__Internal")]    
	private static extern void getAppsFlayerUID(); 

	[DllImport("__Internal")]    
	private static extern void trackLogin(); 

	[DllImport("__Internal")]    
	private static extern void trackRecharge(string info); 
	
	#elif UNITY_ANDROID
	private static string SDKMANAGERCLASS = "driver.sdklibrary.sdkmanager.SDKManager";
	static AndroidJavaClass agent = null;

	private static AndroidJavaClass getAgent ()
	{
		if (agent == null) {
			agent = new AndroidJavaClass (SDKMANAGERCLASS);
		}
		return agent;
	}

	private AndroidJavaObject sdkManager;

	public void setSparklingGameSDKManagerObject (AndroidJavaObject sdkmanager)
	{
		sdkManager = sdkmanager;
	}
	#elif UNITY_WP8
	
	#endif

	//	appsflyer 追踪
	//string contentType = "contentType";
	//string contentId = "contentId";
	//string revenue = "666";
	//string currency = "currency";

	private static SparklingGameSDKManager sparklingGameSDKManager = null;

	public static SparklingGameSDKManager getSparklingGameSDKManager ()
	{
		if (sparklingGameSDKManager == null) {
			sparklingGameSDKManager = new SparklingGameSDKManager ();
				
			//if the platform is real device
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor) {
				#if UNITY_IPHONE

				#elif UNITY_ANDROID
				AndroidJavaObject jobj = getAgent ().CallStatic<AndroidJavaObject> ("getInstance");
				sparklingGameSDKManager.setSparklingGameSDKManagerObject (jobj);
				#elif UNITY_WP8
				#endif
			}
		}
		return sparklingGameSDKManager;
	}

	public void appInit (string appInitGameObj)
	{
		//if the platform is real device
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor) {
			#if UNITY_IPHONE
			notification (appInitGameObj);
			#elif UNITY_ANDROID
			#elif UNITY_WP8
			#endif
		}
	}

	public void showLogin (string loginBackBindUGameObj)
	{
		//if the platform is real device
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor) {
			#if UNITY_IPHONE

			login1(loginBackBindUGameObj);

			#elif UNITY_ANDROID
			if (sdkManager != null) {
				sdkManager.Call ("showLogin", loginBackBindUGameObj);
			}
			#elif UNITY_WP8
			#endif
		}
	}

	public void showPay (string payParamsJson, string payBackBindUGameObj)
	{
		//if the platform is real device
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor) {
			#if UNITY_IPHONE
//			string prama = "{\"cp_orderId\":\""+cp_orderId+"\",\"productId\":\""+productId+"\",\"productName\":\""+productName+"\",\"iProductCount\":\""+iProductCount+"\",\"iPayMoney\":\""+iPayMoney+"\",\"notifyUrl\":\""+notifyUrl+"\",\"appleProductId\":\""+appleProductId+"\",\"extra\":\""+extra+"\",\"iServerId\":\""+iServerId+"\",\"roleId\":\""+roleId+"\",\"characterId\":\""+characterId+"\"}";
//			payClick (payBackBindUGameObj,payParamsJson);

			payClick (payBackBindUGameObj,payParamsJson);

			#elif UNITY_ANDROID
			if (sdkManager != null) {
				sdkManager.Call ("showPay", payParamsJson, payBackBindUGameObj);
			}
			#elif UNITY_WP8
			#endif
		}
	}

	public void loginOff ()
	{
		//if the platform is real device
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor) {
			#if UNITY_IPHONE

			logout();

			#elif UNITY_ANDROID
			if (sdkManager != null) {
				sdkManager.Call ("loginOff");
			}
			#elif UNITY_WP8
			#endif
		}
	}
	
	public void setLanguage (Language language)
	{
		//if the platform is real device
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor) {
			#if UNITY_IPHONE
			#elif UNITY_ANDROID
			if (sdkManager != null) {
				string androidL = "";
				switch (language) {
				case Language.AUTO:
					androidL = "0";
					break;
				case Language.ENGLISH:
					androidL = "1";
					break;
				case Language.CHINESE:
					androidL = "2";
					break;
				}
				sdkManager.Call ("setLanguage", androidL);
			}
			#elif UNITY_WP8
			#endif
		}
	}
}
