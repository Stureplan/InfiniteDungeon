// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Mobile"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SnowTex ("Snow", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True" }

		Blend SrcAlpha OneMinusSrcAlpha
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


			sampler2D _MainTex;
			sampler2D _SnowTex;
			float4 _MainTex_ST;

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

				

				o.normal = v.normal;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = fixed2(y, y);


				//useless atm (pass normals?)
				o.color = fixed4(0, 0 ,0, 0);

				o.normal.xy = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv) * i.color;
				//fixed4 col = i.color;
				//fixed2 uv = fixed2(i.vertex.y, i.vertex.y);
				
				//TODO: Store normal maps. UVs should be UVs, normals normals etc. Height can be a fixed.
				//col.xyz *= dot(fixed3(1, 1, 0), i.normal);


				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(_SnowTex, i.normal.xy);
				
				
				fixed h = 0;
				h = step(0.4, i.uv.x);
				col2 = step(0.65, col2.a) * h;


				return col + col2;
			}
			ENDCG
		}
	}
	CustomEditor "ColorRampShaderGUI"
}
