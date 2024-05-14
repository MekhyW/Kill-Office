using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Adinmo;

///////////////////////////////////////////////////////////////////
// This class shows an example of how callbacks can be used
// to know when the textures have been applied.
//
// This simple example hides the image until it is ready
//
///////////////////////////////////////////////////////////////////
public class ExampleCallbacks : MonoBehaviour {

	public AdinmoTexture m_imageToReplace;
	public bool m_bHideObjectUntilReady = true;

	///////////////////////////////////////////////////////////////////
	// Register callbacks
	///////////////////////////////////////////////////////////////////
	void Start () 
	{
		// Global callback
		AdinmoManager.SetOnReadyCallback( OnAllTexturesDownloaded );
		// Callback per image if desired
		m_imageToReplace.SetOnReadyCallback( OnTextureReplace );
		m_imageToReplace.SetOnFailCallback( OnTextureFail );	

		// Optionally hide the image until it is ready to be shown
		if (m_bHideObjectUntilReady)
			m_imageToReplace.gameObject.SetActive( false );
	}


	///////////////////////////////////////////////////////////////////
	// Once all textures have finished downloading
	///////////////////////////////////////////////////////////////////
	void ShowImage()
	{
		if (m_bHideObjectUntilReady)
			m_imageToReplace.gameObject.SetActive( true );
	}

	///////////////////////////////////////////////////////////////////
	// Once all textures have finished downloading
	///////////////////////////////////////////////////////////////////
	void OnAllTexturesDownloaded( string msg )
	{
		Debug.Log("Ready: " + msg );

		ShowImage();
	}
	

	///////////////////////////////////////////////////////////////////
	// Once this 
	///////////////////////////////////////////////////////////////////
	void OnTextureReplace( AdinmoTexture t )
	{
		Debug.Log("This texture has been replaced.");

		ShowImage();
	}


	///////////////////////////////////////////////////////////////////
	// Once all textures have finished downloading
	///////////////////////////////////////////////////////////////////
	void OnTextureFail( AdinmoTexture t )
	{
		Debug.Log("Texture not replaced -- Original texture will be seen");

		ShowImage();
	}
}
