using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meliasoft.Models
{
    public class JsonItem
    {
        
        public string text;
        public string values;
        public string goals;
        public int borderRadiusTopLeft = 7;
        public double alpha = 0.85; //Transparency
        public string backgroundColor = "";
        public HoverState hoverState;
        public int maxTrackers;
        public string barWidth;
        public string offsetValues;

        public string type;
        public string aspect;
        public bool contourOnTop;

        public int dataSide;

        //Sample data: [{"values": [70,50,75,51,70,73,67,46], "background-color": "#2B2836", "alpha": 0.5, "hover-state": {   "visible": false }, "max-trackers": 0  },  
        //              { "values": [73,77,91,86,67,76,88,96], "background-color": "#C42E53", "alpha": 0.9, "bar-width": "40%"  }]

        public JsonItem(string _text, string _values, double _alpha = 1.0, string _backgroundColor = "", string _goals = "", bool _hoverState = true, int _maxTrackers = 0, string _barWidth = "")
        {
            this.text = _text;
            this.values = _values;
            this.goals = _goals;
            this.alpha = _alpha;
            this.backgroundColor = _backgroundColor;
            this.hoverState = new HoverState(_hoverState);
            this.maxTrackers = _maxTrackers;
            this.barWidth = _barWidth;
            //"borderRadiusTopLeft": 7,
        }

        public JsonItem(string _text, string _values, string _offsetValues, double _alpha = 1.0, string _backgroundColor = "", string _goals = "", bool _hoverState = true, int _maxTrackers = 0, string _barWidth = "")
        {
            this.text = _text;
            this.values = _values;
            this.offsetValues = _offsetValues;
            this.alpha = _alpha;
            this.backgroundColor = _backgroundColor;
            this.goals = _goals;
            this.hoverState = new HoverState(_hoverState);
            if (_maxTrackers >= 0)
                this.maxTrackers = _maxTrackers;
            this.barWidth = _barWidth;
          
        }

        public JsonItem(string _text, string _values, string _goals)
        {
            this.text = _text;
            if (!string.IsNullOrEmpty(_values))
                this.values = _values;
            this.goals = _goals;
        }

        public JsonItem(string _text, string _values, string _offsetValues, double _alpha = 1.0)
        {
            this.text = _text;
            if (!string.IsNullOrEmpty(_values))
                this.values = _values;
            this.offsetValues = _offsetValues;
            this.alpha = _alpha;
        }

        //public JsonItem(string _text, string _values, string _offsetValues, string _backgroundColor = "", double _alpha = 1.0)
        //{
        //    this.text = _text;
        //    if (!string.IsNullOrEmpty(_values))
        //        this.values = _values;
        //    this.offsetValues = _offsetValues;
        //    this.backgroundColor = _backgroundColor;
        //}


        public JsonItem(string _text, string _values, string _type, string _aspect, bool _contourOnTop = true, string _barWidth = "")
        {
            this.text = _text;
            this.values = _values;
            this.type = _type;
            this.aspect = _aspect;

            this.contourOnTop = _contourOnTop;
            this.barWidth = _barWidth;
        }

        //[{ "data-side": 1, "values": [1656154, 1787564 ]},{ "data-side": 2, "values": [1656154, 1787564 ]} ]
        public JsonItem(int _dataSide, string _values) //, bool isPopPyramid, double _alpha = 1.0, string _backgroundColor = ""
        {
            //if (isPopPyramid)
            //{
            this.dataSide = _dataSide;
            this.values = _values;
            //this.alpha = _alpha;
            //this.backgroundColor = _backgroundColor;
            //}
        }

        public void SetBackColor(string _backgroundColor = "")
        {        
            this.backgroundColor = _backgroundColor;
        }


    }

    public class BarJsonItem
    {

        public string values;


        public BarJsonItem(string _values)
        {
            if (!string.IsNullOrEmpty(_values))
                this.values = _values;
     
        }

        public void AddValue(string _value)
        {
            if (!string.IsNullOrEmpty(_value))
            {
                if (!string.IsNullOrEmpty(this.values))
                {
                    this.values = this.values.Replace("[", "").Replace("]", "");
                    this.values += "," + _value;
                    this.values = "[" + this.values + "]";
                } else
                {
                    this.values = "[" + _value + "]";
                }
            }
                
        }

    }
}