// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Mobile"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		
		[Header(Color Properties)]
		_TopColor("Top Color", Color) = (1, 1, 1, 1)
		_MidColor("Mid Color", Color) = (0.2, 0.2, 0, 1)
		_LowColor("Low Color", Color) = (0, 1, 1, 1)
		[Space(10)]
		_TopValue("Top Level", Float) = 0.9
		_MidValue("Mid Level", Float) = 0.75
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
				fixed4 color : COLOR;
				fixed3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				fixed3 normal : NORMAL;
			};


			fixed4 _TopColor;
			fixed4 _MidColor;
			fixed4 _LowColor;

			fixed _TopValue;
			fixed _MidValue;



			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 heightColor(fixed h)
			{
				//fixed4 c1 = _LowColor;
				//fixed4 c2 = _MidColor * step(_MidValue, h);
				//fixed4 c3 = _TopColor * step(_TopValue, h);

				fixed4 c1 = _LowColor * (1-h);
				fixed4 c2 = _MidColor * (_MidValue - h);
				fixed4 c3 = _TopColor * (_TopValue - h);

				return c1 + c2 + c3;
			}

			fixed normalize01(fixed v, fixed min, fixed max)
			{
				fixed n = (v - min) / (max - min);
				return n;
			}

			v2f vert (appdata v)
			{
				v2f o;

				fixed3 wPos = mul(unity_ObjectToWorld, v.vertex);
				fixed y = wPos.y;


				//half nDot = dot(v.normal, fixed3(1, 1, 0));
				//nDot = clamp(nDot, 0.8, 1);
				//fixed4 c = lerp(_LowColor, _TopColor, y);
				//c *= 1 * nDot;

				
				fixed4 c = heightColor(y);


				o.normal = v.normal;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = fixed2(y, y);


				o.color = c;

				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv) * i.color;
				//fixed4 col = i.color;
				//col.xyz *= dot(fixed3(1, 1, 0), i.normal);

				fixed2 uv = fixed2(i.vertex.y, i.vertex.y);
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
	CustomEditor "ColorRampShaderGUI"
}
