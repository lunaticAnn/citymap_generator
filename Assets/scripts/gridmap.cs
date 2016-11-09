using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class gridmap {
	/*
	 * This city generator will leave a blank edge to mark out the city area.
	 * Smallest one will have only one building placing grid which looks like	 
	 */

	const float Thread=200f;

	public List<quand_area> areas;
	public float width;
	public float height;



	public gridmap(float width, float height){

		this.height=height;
		this.width=width;
		this.areas=new List<quand_area>();

	}

	//draw the basic area of the city

	/*
	 * In this part the recursive split will have a depth k
	 * 
	*/
	public enum orientation{v,h};

	public struct quand_area{
		/*
		 * a quand area will be like:
		 * corner10---------corner11
		 * |						\
		 * |						 \
		 * corner00------------corner01
		*/
		public Vector2 corner00;
		public Vector2 corner01;
		public Vector2 corner10;
		public Vector2 corner11;
		public orientation o;
	}




	private Vector2[] split_picker(ref quand_area a){
		float len_h0=Vector2.Distance(a.corner00, a.corner01);
		float len_h1=Vector2.Distance(a.corner10,a.corner11);
		float len_v0=Vector2.Distance(a.corner00,a.corner10);
		float len_v1=Vector2.Distance(a.corner01,a.corner11);

		//judge whether we could split(estimated area bigger than 7*7)
		float est_area=(len_h0+len_h1)*(len_v0*len_v1)/4;
		if (est_area<Thread){
			Debug.LogWarning("Area is too small for spliting");
			areas.Add(a);
			Debug.Log(areas.Count);return null;}
		else{
			Vector2[] start_end=new Vector2[2];
			if((len_h0+len_h1)>(len_v0+len_v1)){
				//split vertically
				a.o=orientation.v;
				float noise=0.1f-0.2f*Random.value;
				Vector2 mid0=(a.corner00+a.corner01)/2+noise*(a.corner01-a.corner00);
				noise=0.1f-0.2f*Random.value;
				Vector2 mid1=(a.corner10+a.corner11)/2+noise*(a.corner11-a.corner10);
				start_end[0]=mid0;
				start_end[1]=mid1;
				return start_end;
			}
			else{
				//split horizontally
				a.o=orientation.h;
				float noise=0.15f-0.3f*Random.value;
				Vector2 mid0=(a.corner00+a.corner10)/2+noise*(a.corner10-a.corner00);
				noise=0.15f-0.3f*Random.value;
				Vector2 mid1=(a.corner01+a.corner11)/2+noise*(a.corner11-a.corner01);
				start_end[0]=mid0;
				start_end[1]=mid1;
				return start_end;
			}
		}
	}

	private quand_area[] new_areas(quand_area a, Vector2 start_pos,Vector2 end_pos,float width){
		quand_area[] new_a=new quand_area[2];
		switch (a.o){
		case orientation.v:
			quand_area a1v=new quand_area();
			a1v.corner00=a.corner00;
			a1v.corner10=a.corner10;
			Vector2 dir_v1=(a.corner01-a.corner00).normalized;
			Vector2 dir_v2=(a.corner11-a.corner10).normalized;

			a1v.corner01=start_pos-0.5f*width*dir_v1;
			a1v.corner11=end_pos-0.5f*width*dir_v2;
			new_a[0]=a1v;
			quand_area a2v=new quand_area();
			a2v.corner00=start_pos+0.5f*width*dir_v1;
			a2v.corner10=end_pos+0.5f*width*dir_v2;
			a2v.corner01=a.corner01;
			a2v.corner11=a.corner11;
			new_a[1]=a2v;
			return new_a;
		case orientation.h:
			quand_area a1h=new quand_area();
			Vector2 dir_h1=(a.corner10-a.corner00).normalized;
			Vector2 dir_h2=(a.corner11-a.corner01).normalized;
			a1h.corner00=a.corner00;
			a1h.corner10=start_pos-0.5f*width*dir_h1;
			a1h.corner01=a.corner01;
			a1h.corner11=end_pos-0.5f*width*dir_h2;
			new_a[0]=a1h;
			quand_area a2h=new quand_area();
			a2h.corner00=start_pos+0.5f*width*dir_h1;
			a2h.corner10=a.corner10;
			a2h.corner01=end_pos+0.5f*width*dir_h2;
			a2h.corner11=a.corner11;
			new_a[1]=a2h;
			return new_a;
		default:
			Debug.LogWarning("no new area added, the previoud area is not updated or not qualified for splitting.");
			return null;
			
		}

	}


	public void road_generator()
	{
		/* the road generator will generate the roads in the input square area
		 * What does it do:
		 * find the ideal split orientation for the area(split on the longest edge);
		 * get the start point and end point;
		 * add some noise(should be in restriction)
		 * get_gridpos(startpos,endpos) 
		 * return the new generated square areas
		*/
		gridmap.quand_area a=new gridmap.quand_area();
		a.corner00=new Vector2(0,0);
		a.corner01=new Vector2(width,0);
		a.corner10=new Vector2(0,height);
		a.corner11=new Vector2(width,height);

		Queue<quand_area> areas=new Queue<quand_area>();

		areas.Enqueue(a);
		int depth_thread=20;

		while(areas.Count!=0){
			quand_area current=areas.Dequeue();
			Vector2[] start_end=split_picker(ref current);
			if(start_end!=null){
			Vector2 startpos=start_end[0];
			Vector2 endpos=start_end[1];
				float road_width;
				if(depth_thread>14){
					road_width=3f;
					}
				else if(depth_thread>0){
					road_width=2f;
					}
				else{
					road_width=1f;
					}
					
				quand_area[] new_a=new_areas(current,startpos,endpos,road_width);
				depth_thread-=1;
				areas.Enqueue(new_a[0]);
				areas.Enqueue(new_a[1]);
			}
		}

	}



}


