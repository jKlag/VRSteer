using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TechTweaking.Bluetooth;
using TechTweaking.BtCore;

public class VRSteer : MonoBehaviour {

	static float refX;
	static float refY;
	static float refZ;

	static float curX;
	static float curY;
	static float curZ;

	static BluetoothDevice device;

	public static void connectDevice(){
		BluetoothDevice[] devices = BluetoothAdapter.getPairedDevices ();
		if (devices != null) {
			device = devices [0];
			device.setEndByte (10);
			device.ReadingCoroutine = ManageConnection;
			device.connect ();
		}
		refX = 0;
		refY = 0;
		refZ = 0;
	}

	public static void calibrate(){
		refX = curX;
		refY = curY;
		refZ = curZ;
	}

	public static Vector3 getVector(){
		return new Vector3(curX - refX, curY - refY, curZ - refZ);
	}

	public static float getX(){
		return (curX - refX);
	}

	public static float getY(){
		return (curY - refY);
	}

	public static float getZ(){
		return (curZ - refZ);
	}


	static IEnumerator  ManageConnection (BluetoothDevice device)
	{//Manage Reading Coroutine

		while (device.IsReading) {
			if (device.IsDataAvailable) {
				//because we called setEndByte(10)..read will always return a packet excluding the last byte 10. 10 equals '\n' so it will return lines.
				byte [] msg = device.read();
				string content;
				content = System.Text.ASCIIEncoding.ASCII.GetString (msg);
				float prevX = curX;
				if(float.TryParse(content,out curX)){
					float newX = float.Parse (content);
					if (newX < 359f && newX > -359f && Mathf.Abs(newX) > 1f) {
						curX = newX;
					} else {
						curX = prevX;
					}
				}
			}
			yield return null;
		}

	}
}
