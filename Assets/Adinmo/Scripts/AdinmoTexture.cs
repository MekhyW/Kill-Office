using UnityEngine;
using UnityEngine.UI;
using Adinmo.Flatbufs;




////////////////////////////////////////////////////////////////////////////////////
// We create our own namespace to not overlap with your app
////////////////////////////////////////////////////////////////////////////////////
namespace Adinmo {


	public delegate void EventCallback( AdinmoTexture t );

	////////////////////////////////////////////////////////////////////////////////////
	public class AdinmoTexture : AdinmoReplace {
		

		// Implementation
		private Image m_image = null;
        private RawImage m_rawImage = null;
		private EventCallback m_onReadyCallback = null;
		private EventCallback m_onFailCallback = null;
		private GameObject m_debugBackground = null;
		private Image m_debugBackgroundImage = null;
        private GameObject m_debugForeground = null;
        private Image m_debugForegroundImage = null;
        private static bool s_bShowMessageFromServer = true;

        private const int BORDER_WIDTH= 3;
		////////////////////////////////////////////////////////////////////////////////////
		void Start() 
		{
			ShowMessageFromServer();
			Init();
		}
		
		
		////////////////////////////////////////////////////////////////////////////////////
		// Allow the server to print a debug message to the editor window just once.
		// This message is usually a notification that the SDK is oboslete
		////////////////////////////////////////////////////////////////////////////////////
		void ShowMessageFromServer()
		{
			if (s_bShowMessageFromServer)
			{
				s_bShowMessageFromServer = false;
				
				string message = AdinmoManager.GetMessageFromServer();
				if (message != null && message != "")
					Debug.Log( message );				
			}
		}


		////////////////////////////////////////////////////////////////////////////////////
		protected override void ApplyTextureToImage(AdinmoBaseTexture id, Texture t,Placement placement=null)
		{
			Sprite imageSprite = null;
            if (id.MediaType != MediaType.Video)
            {
                if (m_rawImage != null)
                {
                    Color imageColour = m_rawImage.color;
                    Material mat = m_rawImage.material;
                    bool raycastTarget = m_rawImage.raycastTarget;
                    DestroyImmediate(m_rawImage);
                    m_rawImage= null;
                    m_image = gameObject.AddComponent<Image>();
                    m_image.color = imageColour;
                    m_image.material = mat;
                    m_image.raycastTarget = raycastTarget;
                }
                // Remember the original texture
                if (m_image.sprite != null)
                    m_originalTexture = m_image.sprite.texture;
				if (imageSprite != null)
				{
					UnityEngine.Object.Destroy(imageSprite);
					imageSprite = null;
				}
				if (placement!=null)
                {
					m_image.overrideSprite = placement.GetOrCreateImageSprite(id.Guid, (Texture2D)t);
                }
				else
                {
					imageSprite = Sprite.Create((Texture2D)t, new Rect(0, 0, t.width, t.height), new Vector2(.5f, .5f), 100.0f, 0, SpriteMeshType.FullRect);
					m_image.overrideSprite = imageSprite;
				}

            }
            else
            {
                if (m_image != null)
                {
                    Color imageColour = m_image.color;
                    Material mat = m_image.material;
                    bool raycastTarget = m_image.raycastTarget;
                    DestroyImmediate(m_image);
                    m_image = null;
                    m_rawImage = gameObject.AddComponent<RawImage>();
                    m_rawImage.color = imageColour;
                    m_rawImage.material = mat;
                    m_rawImage.raycastTarget = raycastTarget;
                }
                m_rawImage.texture = t;
            }

        }


		////////////////////////////////////////////////////////////////////////////////////
		public override bool IsImageActive()
		{
            if (m_rawImage)
                return m_rawImage.isActiveAndEnabled;
            else
                return m_image.isActiveAndEnabled;
		}

		////////////////////////////////////////////////////////////////////////////////////
		public override void Init() 
		{
			// Get the image component
			m_image = GetComponent<Image>();
			if (m_image != null)
				m_orignalImage = m_image.gameObject;
			else
				m_orignalImage = null;

			base.Init();
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void SetOnReadyCallback( EventCallback pfn )
		{
			m_onReadyCallback = pfn;
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void SetOnFailCallback( EventCallback pfn )
		{
			m_onFailCallback = pfn;
		}

		////////////////////////////////////////////////////////////////////////////////////
		protected override void OnReady()
		{
			// Let the owner know that this is ready with the downloaded texture
			if (m_onReadyCallback != null)
				m_onReadyCallback( this );
		}

		////////////////////////////////////////////////////////////////////////////////////
		protected override void OnFail()
		{
			// Let the owner know that this is ready with the downloaded texture
			if (m_onFailCallback != null)
				m_onFailCallback( this );
		}


		////////////////////////////////////////////////////////////////////
		protected override void SetRendererEnabled( bool bEnable )
		{
			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			if (sr != null)
				sr.enabled = bEnable;
			else
			{
				MeshRenderer mr = GetComponent<MeshRenderer>();
				if (mr != null)
					mr.enabled = bEnable;

				else
				{
					Image i = GetComponent<Image>();
					if (i != null)
						i.enabled = bEnable;
				}
			}
		}


		////////////////////////////////////////////////////////////////////////////////////
		// Start drawing a red or green border around the image to say whether it is big enough
		////////////////////////////////////////////////////////////////////////////////////
		public override void UpdateDebugging( Color c, float percentRemaining )
		{		
			base.UpdateDebugging( c, percentRemaining );

			if (m_debugBackground != null && m_debugBackgroundImage != null)
			{
				DuplicateParentWidth(percentRemaining);
				m_debugBackgroundImage.color = c;
                m_debugForegroundImage.enabled = (c == Color.white);
			}
		}


		////////////////////////////////////////////////////////////////////////////////////
		// Start drawing a red or green border around the image to say whether it is big enough
		////////////////////////////////////////////////////////////////////////////////////
		public override void StartDebugging()
		{		
			base.StartDebugging();

			// Quads and Sprites are handled by the base class
			Image i = GetComponent<Image>();
			if (i == null)
				return;

			GameObject borderPrefab = (GameObject)Resources.Load("AdinmoDebugImage", typeof(GameObject));

			if (borderPrefab == null)
				return;

			m_debugBackground = Instantiate(borderPrefab, transform);
            m_debugBackground.name = "Debug Background";
			m_debugBackgroundImage = m_debugBackground.GetComponent<Image>();
            m_debugForeground = Instantiate(borderPrefab, transform);
            m_debugForeground.name = "Debug Foreground";
            m_debugForegroundImage = m_debugForeground.GetComponent<Image>();
            RectTransform fgRt = m_debugForeground.GetComponent<RectTransform>();
            fgRt.position += (Vector3.down+Vector3.right) * BORDER_WIDTH;

            DuplicateParentWidth(0);
		}

		////////////////////////////////////////////////////////////////////////////////////
		protected void DuplicateParentWidth(float percentRemaining)
		{
			RectTransform rt = GetComponent<RectTransform>();
			if (rt == null)
				return;
			
			RectTransform bgRt = m_debugBackground.GetComponent<RectTransform>();
			if (bgRt == null)
				return;
			bgRt.sizeDelta = new Vector2( rt.rect.width, 20 );
            RectTransform fgRt = m_debugForeground.GetComponent<RectTransform>();
            if (fgRt == null)
                return;
            float fgWidth = (rt.rect.width-BORDER_WIDTH*2) * percentRemaining;
            fgRt.sizeDelta = new Vector2(fgWidth, 20-BORDER_WIDTH*2);
            
        }
			
		////////////////////////////////////////////////////////////////////////////////////
		// Stop drawing a red or green border around the image to say whether it is big enough
		////////////////////////////////////////////////////////////////////////////////////
		public override void StopDebugging()
		{		
			base.StopDebugging();

			if (m_debugBackground)
				Destroy(m_debugBackground);
		}


	}

}
