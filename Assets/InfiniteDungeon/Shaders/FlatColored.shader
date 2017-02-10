// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/FlatColored"
{
	Properties
	{
		_MainColor ("Main Color", Color) = (1, 1, 1, 1)
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }


		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				fixed3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				fixed3 normal : NORMAL;
			};

			fixed4 _MainColor;


			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.normal = v.normal;


				//debug
				//o.color = _MainColor;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.color;

				return col;
			}
			ENDCG
		}
	}
}
