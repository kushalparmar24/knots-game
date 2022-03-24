using UnityEngine;
using System.Collections;

[System.Serializable]
public class ArrayLayout  {

	public int ysize,xsize = 3;
	[System.Serializable]
	public struct rowData{
		public int[] row;
	}

	public rowData[] rows = new rowData[7]; //Grid of 7x7

	public void changesize(int ysize_, int xsize_)
    {
		xsize = xsize_;
		ysize = ysize_;
		rows = new rowData[xsize];
		

	}
}
