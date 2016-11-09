using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour {

	const float cube_edge=1.0f;
	public GameObject building;
	public int width=3;
	public int height=3;
	gridmap m;
	// Use this for initialization
	void Start () {
		m=new gridmap(100f,100f);

		m.road_generator();
		show_grids(Vector3.zero,m);

	
	}
	



	float height_calc(float x, float y)
	{
		float midh=height/2f;
		float midw=width/2f;
		//use manhattan distance as the factor
		float factor=Mathf.Abs(x-midw)+Mathf.Abs(y-midh)+0.1f*(1f+0.2f*Random.value)*(height+width);
		return 5*(height+width)/factor;
	}


	void show_grids(Vector3 center_point,gridmap m){
		Debug.Log(m.areas.Count);
		GameObject city=new GameObject("city");
		foreach(gridmap.quand_area a in m.areas){
			GameObject new_building=Instantiate(building);
			new_building.transform.SetParent(city.transform);

			Vector2[] generator={a.corner10,a.corner11,a.corner00,a.corner01};

			Vector2 sum=Vector2.zero;
			foreach(Vector2 v in generator){
				sum+=v;
			}
			sum=sum/4;
			Mesh cube_mesh=new_building.GetComponent<MeshFilter>().mesh;
			float h=height_calc(sum.x,sum.y);
			change_cube(cube_mesh,generator,h);
		}
		

	}

	void change_cube(Mesh cube_mesh,Vector2[] quad_corners,float h=1f){

		Vector3[] vs=cube_mesh.vertices;
		cube_mesh.vertices=change_my_cube(ref vs,quad_corners,h);
		cube_mesh.RecalculateBounds();
	}


	Vector3[] change_my_cube(ref Vector3[] cube_vertices,Vector2[] map_area,float h=1f){
		/*this will map the cube vertices to a shape of map_area
		 * which looks like:(easy to fit quad-areas in map
		 *    0------1
		 *   /       |
		 *  /        |
		 * 2---------3
		*will return new vertices according to old template
			*/
		//create template
		Dictionary<Vector3,Vector3> v_convert=new Dictionary<Vector3,Vector3>();
		//bottom_left_back(0)
		v_convert[new Vector3(0.5f,-0.5f,0.5f)]=new Vector3(map_area[0].x,0f,map_area[0].y);
		//top_left_back(0)
		v_convert[new Vector3(0.5f,0.5f,0.5f)]=new Vector3(map_area[0].x,h,map_area[0].y);
		//bottom_right_back(1)
		v_convert[new Vector3(0.5f,-0.5f,-0.5f)]=new Vector3(map_area[1].x,0f,map_area[1].y);
		//top_right_back(1)
		v_convert[new Vector3(0.5f,0.5f,-0.5f)]=new Vector3(map_area[1].x,h,map_area[1].y);
		//bottom_left_front(2)
		v_convert[new Vector3(-0.5f,-0.5f,0.5f)]=new Vector3(map_area[2].x,0f,map_area[2].y);
		//top_left_front(2)
		v_convert[new Vector3(-0.5f,0.5f,0.5f)]=new Vector3(map_area[2].x,h,map_area[2].y);
		//bottom_right_front(3)
		v_convert[new Vector3(-0.5f,-0.5f,-0.5f)]=new Vector3(map_area[3].x,0f,map_area[3].y);
		//top_right_font(3)
		v_convert[new Vector3(-0.5f,0.5f,-0.5f)]=new Vector3(map_area[3].x,h,map_area[3].y);
		Vector3[] new_vertices=cube_vertices;
		for(int i=0;i<new_vertices.GetLength(0);i++){
			new_vertices[i]=v_convert[new_vertices[i]];
		}
		return new_vertices;

	}

}
