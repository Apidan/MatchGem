using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTexture : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
        WWW www = new WWW("https://gimg2.baidu.com/image_search/src=http%3A%2F%2Fcdn.duitang.com%2Fuploads%2Fblog%2F201306%2F25%2F20130625150506_fiJ2r.jpeg&refer=http%3A%2F%2Fcdn.duitang.com&app=2002&size=f9999,10000&q=a80&n=0&g=0n&fmt=jpeg?sec=1626509905&t=aaeb58897a3f20dce72871185d18d5c5");
        yield return www;
        Debug.Log(www.error);
        
        if (www!=null)
        {
                Texture texture = www.texture;
            GetComponent<Renderer>().material.mainTexture = texture;
        }

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Texture texture = Resources.Load("Pic")as Texture;
            GetComponent<Renderer>().material.mainTexture = texture;
        }
	}
}
