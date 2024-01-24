shader_type spatial;
render_mode cull_disabled;

uniform sampler2D HeightmapTexture : repeat_disable,filter_linear;
uniform vec3 worldSize;
uniform vec3 terrainPos;

varying vec2 _textureUV;
varying vec3 _worldVertex;


void vertex() {
	
	_worldVertex=VERTEX+MODEL_MATRIX[3].xyz-terrainPos*vec3(2,2,2);
	//_worldVertex=VERTEX+MODEL_MATRIX[3].xyz;
	
	vec2 heightmapSize=worldSize.xz;
	_textureUV=(_worldVertex.xz+(worldSize.xz*0.5)+0.5)/(worldSize.xz+1.0);
	float height=textureLod(HeightmapTexture,_textureUV,0.0).r;
	VERTEX.y=height*worldSize.y;
}
