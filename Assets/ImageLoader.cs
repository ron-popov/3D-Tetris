using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour {

	public TextAsset imageAsset;

	// Use this for initialization
	void Start () {
		Texture2D texture = new Texture2D(1000, 500, TextureFormat.RGB24, false);
		Image image = GameObject.Find("Start Menu/Canvas/Image").GetComponent<Image>();
		texture.LoadImage(imageAsset.bytes);
		image.material.mainTexture = texture;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
