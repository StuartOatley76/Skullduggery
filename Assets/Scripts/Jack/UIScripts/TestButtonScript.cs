using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButtonScript : MonoBehaviour
{
	public void TestButtonFunction()
	{
		Debug.Log("Button with name: '" + gameObject.name + "' has been invoked.");
	}
}
