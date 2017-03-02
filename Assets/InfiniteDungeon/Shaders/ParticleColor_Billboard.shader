Shader "InfiniteDungeon/ParticleColor_Billboard"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)

		_ScaleX("Scale X", Float) = 1
		_ScaleY("Scale Y", Float) = 1
	}

	SubShader
	{
		Tags
		{ 
			"Queue" = "Overlay" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent" 
			"PreviewType" = "Plane"
		}

		Blend SrcAlpha One
		Cull Off Lighting Off ZWrite Off

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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			fixed4 _Color;

			fixed _ScaleX;
			fixed _ScaleY;


			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0)) - fixed4(v.vertex.x, v.vertex.y, 0.0, 0.0) * fixed4(_ScaleX, _ScaleY, 1.0, 1.0));
				
				
				
				o.uv = v.uv;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed3 col2 = _Color.rgb;
				return fixed4(col2.rgb, col.a);
			}
			ENDCG
		}
	}
}
