using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparklingGameDemo : MonoBehaviour
{
	//ios&android
	SparklingGameSDKManager sparklingGameSDKManager;

	void Start ()
	{	
		//ios&android
		sparklingGameSDKManager = SparklingGameSDKManager.getSparklingGameSDKManager ();
		sparklingGameSDKManager.setLanguage (Language.ENGLISH);

		//ios
		//ios 通知调用
		sparklingGameSDKManager.appInit ("Main Camera");
	}

	void OnGUI ()
	{
		int left = Screen.width / 10;
		int height = Screen.height / 8;
		int top = Screen.height / 5;
		int setp = Screen.height / 4;
		int width = Screen.width - left * 2;
		int i = 0;
			
		GUI.skin.button.fontSize = height / 5 * 3;
		GUI.skin.box.fontSize = height / 4;
		GUI.Box (new Rect (0, 0, Screen.width, Screen.height), "SparklingGameDemo Menu");

		if (GUI.Button (new Rect (left, top + setp * i++, width, height), "登录")) {
			//ios&android
			sparklingGameSDKManager.showLogin ("Main Camera");
		}

		if (GUI.Button (new Rect (left, top + setp * i++, width, height), "充值")) {
			//ios&android
			string payParamsJson = "{\"key\":\"61a0411511d3b1866f\"," +
				"\"player_id\":\"10154\",\"server_id\":\"13\"," +
				"\"character_id\":\"10240\",\"product_id\":\"sp00002\"," +
				"\"product_name\":\"钻石\",\"product_count\":\"121\"," +
				"\"cp_order_number\":\"0304050607\",\"trans\":\"sdea\"," +
				"\"notify_url\":\"http://localhost/oldtest/index.php\"," +
				"\"total\":\"100\"}";
			sparklingGameSDKManager.showPay (payParamsJson, "Main Camera");
		}
			
		if (GUI.Button (new Rect (left, top + setp * i++, width, height), "注销")) {
			//ios&android
			sparklingGameSDKManager.loginOff ();
		}
	}

	//ios
	//sdk初始化成功给cp回调
	public void initSuccess (string jsonUser)
	{
		Debug.Log ("initSuccess:" + jsonUser);
	}
	//ios
	//sdk 初始化失败给cp的回调
	public void InitFail (string jsonUser)
	{
		Debug.Log ("InitFail:" + jsonUser);

	}

	//ios&android
	//	用户登录成功给cp回调
	public void onLoginSuccess (string jsonUser)
	{	
		Debug.Log ("onLoginSuccess:" + jsonUser);
	}

	//ios
	//	用户登录失败给cp回调
	public void onLoginFail (string jsonUser)
	{	
		Debug.Log ("onLoginFail:" + jsonUser);
	}

	//ios
	//	用户登出给cp的回调
	public void logoutSuccess (string jsonUser)
	{	
		Debug.Log ("logoutSuccess:" + jsonUser);
	}

	//ios
	//	用户支付成功给cp的回调
	public void onPaySuccess (string jsonPaySuccess)
	{
		Debug.Log ("onPaySuccess:" + jsonPaySuccess);
	}

	//ios
	//	用户支付失败给cp的回调
	public void onPayError (string jsonPayError)
	{
		Debug.Log ("onPayError:" + jsonPayError);
	}

	//android
	//支付结束
	public void onPayEnd ()
	{
		Debug.Log ("onPayEnd");
	}
}
