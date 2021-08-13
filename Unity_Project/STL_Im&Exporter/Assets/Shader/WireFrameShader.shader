Shader "Custom/WireFrameShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LineColor("Line Color", Color) = (100.0, 100.0, 100.0, 100.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags { "RenderType" = "Opaque"}
			CGPROGRAM
 
			#pragma vertex vert
			#pragma fragment frag
 
			#include "UnityCG.cginc"
 
			struct v2f
			{
				half4 pos : SV_POSITION;
			};
 
			fixed4 _LineColor;
 
			v2f vert(appdata_base  v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
 
			fixed4 frag(v2f i) : COLOR
			{
				return _LineColor;
			}
 
			ENDCG
        }
    }

    FallBack "Diffus"
}
