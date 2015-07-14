﻿/*
 * Copyright (c) 2014, Roger Lew (rogerlew.gmail.com)
 * Date: 5/20/2015
 * License: BSD (3-clause license)
 * 
 * The project described was supported by NSF award number IIA-1301792
 * from the NSF Idaho EPSCoR Program and by the National Science Foundation.
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VTL.TrendGraph
{
    public struct TimeseriesRecord
    {
        public DateTime time;
        public float value;
        public float[,] Data;
        public TimeseriesRecord(DateTime time, float value,float[,] data)
        {
            this.time = time;
            this.value = value;
            this.Data = data;
        }
    }

    public class TrendGraphController : MonoBehaviour
    {
        public Color lineColor = Color.white;
        public float lineWidth = 1f;
        public float yMax = 1;
        public float yMin = 0;
        public float timebase = 300; // in seconds
        public string minTime = ""; // Its on the developer to make sure 
                                                // this makes sense with the timebase
        public string maxTime = ""; // Its on the developer to make sure 
        // this makes sense with the timebase
        public string unitsLabel = "F"; // the units label
        public string variable_name = "";
        public string valueFormatString = "D3";
        private DateTime lastDraw;
		public bool Keep = true;
        List<TimeseriesRecord> timeseries;
        Text valueText;
        Vector3 origin;
        float w;
        float h;
        public int row = 0;
        public int col = 0;
        RectTransform rectTransform;
        RectTransform lineAnchor;
        Canvas parentCanvas;
        public float easting;
        public float northing;
        public Material GraphMaterial;
        public Image GraphImage;

        public void OnValidate()
        {
            transform.Find("Ymax")
                     .GetComponent<Text>()
                     .text = yMax.ToString();

            transform.Find("Ymin")
                     .GetComponent<Text>()
                     .text = yMin.ToString();

            transform.Find("MinHour")
                     .GetComponent<Text>()
                     .text = minTime;

            transform.Find("MaxHour")
                     .GetComponent<Text>()
                     .text = maxTime;

            transform.Find("Units")
                     .GetComponent<Text>()
                     .text = unitsLabel;
        }

        public void SetTime(string min, string max)
        {
            transform.Find("MinHour")
                     .GetComponent<Text>()
                     .text = min;
            minTime = min;

            transform.Find("MaxHour")
                     .GetComponent<Text>()
                     .text = max;
            maxTime = max;
        }

        public void SetMinMax(int min, int max)
        {
            transform.Find("Ymax")
                     .GetComponent<Text>()
                     .text = max.ToString();
            yMax = max;

            transform.Find("Ymin")
                     .GetComponent<Text>()
                     .text = min.ToString();
            yMin = min;
        }

        // Use this for initialization
        void Start()
        {
            timeseries = new List<TimeseriesRecord>();
            rectTransform = transform.Find("Graph") as RectTransform;
            lineAnchor = transform.Find("Graph")
                                  .Find("LineAnchor") as RectTransform;

            // Walk up the Hierarchy to find the parent canvas.
            var parent = transform.parent;
            while(parent != null)
            {
                parentCanvas = parent.GetComponent<Canvas>();
                if (parentCanvas != null)
                    parent = null; // stop condition
                else
                    parent = parent.parent;
            }

            valueText = transform.Find("Value").GetComponent<Text>();

            // Set image material
            GraphImage.material = GraphMaterial;
        }

        // The Drawing.DrawLine method using the GL and GUI class and 
        // has to be drawn in draw line
        void OnGUI()
        {
            // sort the records incase they are out of order
            timeseries.Sort((s1, s2) => s1.time.CompareTo(s2.time));
			//Debug.LogError (timeseries.Count);
            // cull old records

			if (!Keep) {
				var elapsed = (float)(lastDraw - timeseries [0].time).TotalSeconds;
				while (elapsed > timebase && elapsed > 0) { 
					timeseries.RemoveAt (0);
					if (timeseries.Count == 0)
						return;
					elapsed = (float)(lastDraw - timeseries [0].time).TotalSeconds;
				}

				// cull future records
				// e.g. SimTimeControl, user scrubbing backwards
				int m = timeseries.Count - 1;
				if (m == -1)
					return;
				while (timeseries[m].time > (DateTime)lastDraw) {
					timeseries.RemoveAt (m);
					m = timeseries.Count - 1;
					if (m == -1)
						return;
				}
			}

            // return if there are less than 2 records after culling
            int n = timeseries.Count;
            if (n < 2)
                return;

            // Need to check the origin and the width and height every draw
            // just in case the panel has been resized
            switch (parentCanvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    origin = rectTransform.position;
                    origin.y = Screen.height - origin.y;
                    break;
                default:
                    origin = RectTransformUtility.WorldToScreenPoint(parentCanvas.worldCamera, lineAnchor.position);
                    origin.y = Screen.height - origin.y;
                    break;
            }
            w = rectTransform.rect.width * transform.localScale.x;
            h = rectTransform.rect.height * transform.localScale.y;

            /* OLD
            // Iterate through the timeseries and draw the trend segment
            // by segment.
            var prev = Record2PixelCoords(timeseries[0]);
            for (int i = 1; i < n; i+=4)
            {
                var next = Record2PixelCoords2(timeseries[i]);
                Drawing.DrawLine(prev, next, lineColor, lineWidth, false);
                prev = next;
            }
            */
        }

        public void Clear()
        {
            timeseries.Clear();
        }

        public void Compute()
        {
            Debug.LogError("trend Graph Compute Called");
            int width = 290;
            int height = 161;
            Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            Color[] color = new Color[width * height];
            Utilities util = new Utilities();
            for (int i = 0; i < width * height; i++)
            {
                if (i > (width * height)/2)
                {
                    color[i] = new Color(0, 0, 0, 1);
                }
                else
                {
                    color[i] = new Color(0, 0, 0, 0);
                }
            }
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.SetPixels(color);
            tex.Apply();
            //GraphImage.material.SetTexture("_MainTex", tex);
            //GraphMaterial.SetTexture("_MainTex", tex);
            GraphImage.sprite = Sprite.Create(tex, new Rect(0, 0, width, height), Vector2.zero );
            //GraphImage.material.mainTexture = tex;
        }

        // converts a TimeseriesRecord to screen pixel coordinates for plotting
        Vector2 Record2PixelCoords(TimeseriesRecord record)
        {
            float s = (float)(lastDraw - record.time).TotalSeconds;
            float normTime = Mathf.Clamp01(1 - s / timebase);
            float normHeight = Mathf.Clamp01((record.Data[row,col] - yMin) / (yMax - yMin));
            //float normHeight = Mathf.Clamp01((record.value - yMin) / (yMax - yMin));
            return new Vector2(origin.x + w * normTime,
                               origin.y + h * (1 - normHeight));
        }


		Vector2 Record2PixelCoords2(TimeseriesRecord record)
		{
			//float s = (float)(lastDraw - record.time).TotalSeconds;
			//float s = (float)(lastDraw - record.time).TotalSeconds;
			//float normTime = Mathf.Clamp01(1 - s / timebase);
			float normTime = (float)(record.time - Start2).TotalSeconds / (float)(End - Start2).TotalSeconds;
            float normHeight = 0;
            if (record.Data != null)
            {
                normHeight = Mathf.Clamp01((record.Data[row, col] - yMin) / (yMax - yMin));
            }
            //float normHeight = Mathf.Clamp01((record.value - yMin) / (yMax - yMin));
			return new Vector2(origin.x + w * normTime,
			                   origin.y + h * (1 - normHeight));
		}

		public DateTime Start2 = DateTime.MaxValue,End=DateTime.MinValue;

        // add a time series to the trend graph
        public void Add(TimeseriesRecord record)
        {
            timeseries.Add(record);
			if (Start2 > record.time) 
			{
				Start2 = record.time;
			}
		    if (End < record.time) {
				End = record.time;
			}
            lastDraw = record.time;
            valueText.text = record.value.ToString(valueFormatString);
            OnValidate();
        }

        public void SetUnit(string unit)
        {
            variable_name = unit;
            VariableReference newVariable = new VariableReference();
            unitsLabel = newVariable.GetDescription(unit);
            OnValidate();
        }
        
        public void Add(DateTime time, float value,float[,] data)
        {
            Add(new TimeseriesRecord(time, value,data));
        }

        public void SetCoordPoint(Vector3 point)
        {
            easting = point.x;
            northing = point.z;
        }

        public void dataToFile()
        {
            // Temp patch to the OS dependen Compute Shader
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            string pathDownload = pathUser + "\\graph.txt";
#else
		string pathDownload = pathUser + "/slicer_path.txt";
#endif

            float[] csv_file = new float[timeseries.Count];

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@pathDownload))
            {
                file.WriteLine(variable_name + ": " + unitsLabel);
                file.WriteLine("Time Frame: " + minTime + " to " + maxTime);
                file.WriteLine("UTM: (" + easting + ", " + northing + ")");
                file.WriteLine("UTM Zone: " + coordsystem.localzone);
                foreach (var i in timeseries)
                {
                    file.Write(i.Data[row, col] + ", ");
                }
            }
        }
    }
}
