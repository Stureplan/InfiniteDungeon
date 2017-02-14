Shader "InfiniteDungeon/VertexLitColorShaded_Animated"
{
	Properties
	{
		_MainColor ("Main Color", Color) = (1, 1, 1, 1)
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout" "LightMode" = "ForwardBase" }

		Blend SrcAlpha OneMinusSrcAlpha

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
				fixed3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				fixed height : TEXCOORD1;
			};

			fixed4 _MainColor;

			float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
			}


			v2f vert (appdata v)
			{
				v2f o;

				//fixed3 pos = rand(v.vertex) / 100;
				fixed3 pos = fixed3(0, 0, 0);

				fixed r = rand((v.vertex.xyz * _SinTime.r)*0.00001);
				pos.x = (r/ 100);
				pos.y = (r / 500);
				pos.z = (r / 1000);



				v.vertex.xyz += pos;
				o.height = v.vertex.y;
				
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				fixed3 normal = v.normal;


				

				// Apply base color.
				fixed4 col = _MainColor;

				// Hard edges.
				half nDot = dot(normal, fixed3(1, 1, 0));
				nDot = clamp(nDot, 0.8, 1);

				o.color = col;
				o.color.rgb *= nDot;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.color;
				
				fixed c = sin(_Time.w * 10);
				col.g += (c * 0.02 + (rand(i.color * _SinTime) * 0.01));

				fixed y = clamp(i.height, 0.1, 1.0);
				y = max(y, 0.2);
				col.rgb += y;

				return col;
			}
			ENDCG
		}
	}
}
