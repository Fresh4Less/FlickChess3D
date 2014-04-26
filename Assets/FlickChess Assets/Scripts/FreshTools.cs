using UnityEngine;
using System.Collections;

public class FreshTools {

	static public Rect calcPixelPosition(Rect rect)
	{
		return new Rect(rect.x * Screen.width, rect.y * Screen.height, rect.width * Screen.width, rect.height * Screen.height);
	}
}
