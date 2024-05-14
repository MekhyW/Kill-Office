Shader "Adinmo/CombinedShader"
{
    Properties
    {
        _AdsTex ("AdsTexture", 2D) = "white" {}
		_EverythingElseTex ("EverythingElseTexture", 2D) = "white" {}
    }
    SubShader
    {

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _AdsTex;
			sampler2D _EverythingElseTex;
            float4 _AdsTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _AdsTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col1 = tex2D(_AdsTex, i.uv);
				fixed4 col2 = tex2D(_EverythingElseTex, i.uv);
				
				if (col1[0]>col2[0])
				{
					fixed4 col={1.0,1.0,1.0,1.0};
					return col;
                }
				else
				{
					fixed4 col={0.0,0.0,0.0,1.0};
					return col;
				}

            }
            ENDCG
        }
    }
}
