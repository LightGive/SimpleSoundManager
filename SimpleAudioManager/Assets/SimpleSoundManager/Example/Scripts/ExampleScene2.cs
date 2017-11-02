using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScene2 : MonoBehaviour
{
	[SerializeField, Header("落下地点を示すマーク")]
	private GameObject targetObj;

	[SerializeField, Header("球体")]
	private Rigidbody sphere;

	[SerializeField, Header("球が発射される場所")]
	private Transform shotAnchor;

	[SerializeField, Header("滞空時間")]
	private float t;

	private bool isShot = false;

	void Update()
	{
		RaycastHit hit;

		if (isShot)
		{
			if (Input.GetMouseButtonUp(0))
				isShot = false;
			return;
		}

		//マウスからRayを飛ばして当たった場所に飛ばす様にする
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
		{
			targetObj.transform.position = hit.point;
			if (Input.GetMouseButtonUp(0))
			{
				isShot = true;
				sphere.isKinematic = true;
				sphere.transform.position = shotAnchor.transform.position;

				var sp = shotAnchor.transform.position;
				var ep = hit.point;
				var g = -Physics.gravity.y;
				var offset = ep.y - sp.y;

				//ｘ,zの初速を計算する
				var x = (ep.x - sp.x) / t;
				var z = (ep.z - sp.z) / t;

				//yの初速を計算する、発射位置のy座標が0ではないので発射座標分を引く。
				var y = (ep.y / t) + (0.5f * g * t) - (sp.y / t);
				var vec = new Vector3(x, y, z);

				//球体発射
				sphere.isKinematic = false;
				sphere.AddForce(vec, ForceMode.VelocityChange);
			}
		}
	}
}