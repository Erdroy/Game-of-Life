// Erdroy's Game of Life © 2016-2017 Damian 'Erdroy' Korczowski

Shader "Unlit/CellShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("CellColor", COLOR) = (0, 1, 0, 1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				
				// TODO: we need a little bit better borders
				if (i.uv.x < 0.0001f || i.uv.y < 0.0001f || i.uv.x >= 0.9999f || i.uv.y >= 0.9999f)
				{
					col = float4(1.0f, 0.55f, 0.0f, 1.0f);
				}
				else 
				{
					if (col.r > 0.01f)
						col = _Color;
					else
						col = float4(0.0f, 0.0f, 0.0f, 1.0f);
				}

				return col;
			}
			ENDCG
		}
	}
}
